using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using zapat.Data;
using zapat.Models;
using zapat.Services;

namespace zapat.Controllers
{
    
    public class PagoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagoController(ILogger<CatalogoController> logger, ApplicationDbContext context, CurrencyService currencyService, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Create()
        {

            var userIDSession = _userManager.GetUserName(User);
            if (userIDSession == null)
            {
                ViewData["Message"] = "Debe iniciar sesión para realizar un pago.";
                return RedirectToAction("Index", "Catalogo");
            }

            // Obtener todos los ítems pendientes del carrito
            var items = _context.DbSetPreOrden
                        .Where(o => o.UserName == userIDSession && o.Status == "PENDIENTE")
                        .Include(p => p.Producto)
                        .OrderBy(o => o.Id)
                        .ToList();

            // Obtener la moneda seleccionada
            var selectedCurrency = HttpContext.Session.GetString("Currency") ?? "USD";
            decimal rate = 1;

            if (selectedCurrency != "USD")
            {
                rate = await _currencyService.GetExchangeRateAsync("USD", selectedCurrency);
            }

            // Calcular monto total convertido
            decimal total = items.Sum(i => i.Cantidad * i.Precio);
            decimal montoConvertido = Math.Round(total * rate, 2);

            Pago pago = new Pago
            {
                UserName = userIDSession,
                MontoTotal = montoConvertido
            };

            _logger.LogInformation("Monto convertido: {0} {1}", montoConvertido, selectedCurrency);
            ViewBag.Currency = selectedCurrency;

            return View(pago);
        }

        [HttpPost]
        public IActionResult Pagar(Pago pago)
        {
            pago.PaymentDate = DateTime.UtcNow;
            _context.Add(pago);

            var itemsCarrito = from o in _context.DbSetPreOrden select o;
            itemsCarrito = itemsCarrito.
                Include(p => p.Producto).
                Where(s => s.UserName.Equals(pago.UserName) && s.Status.Equals("PENDIENTE"));

            Orden pedido = new Orden();
            pedido.UserName = pago.UserName;
            pedido.Total = pago.MontoTotal;
            pedido.Pago = pago;
            pedido.Status = "PENDIENTE";
            _context.Add(pedido);

            List<DetalleOrden> itemsPedido = new List<DetalleOrden>();
            foreach (var item in itemsCarrito.ToList())
            {
                DetalleOrden detallePedido = new DetalleOrden();
                detallePedido.Orden = pedido;
                detallePedido.Precio = item.Precio;
                detallePedido.Producto = item.Producto;
                detallePedido.Cantidad = item.Cantidad;
                itemsPedido.Add(detallePedido);
            }


            _context.AddRange(itemsPedido);

            foreach (PreOrden p in itemsCarrito.ToList())
            {
                p.Status = "PROCESADO";
            }

            _context.UpdateRange(itemsCarrito);
            _context.SaveChanges();

            pago.Status = "CANCELADO";
            _context.Update(pago);
            _context.SaveChanges();

            

           ViewData["Message"] = "El pago se ha registrado y su pedido nro " + pedido.Id + " está en camino";
            return View("Create", pago);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}