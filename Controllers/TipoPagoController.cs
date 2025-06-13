using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
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
    
    public class TipoPagoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        

        public TipoPagoController(ILogger<CatalogoController> logger, ApplicationDbContext context, CurrencyService currencyService, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(decimal monto)
        {
            var userIDSession = _userManager.GetUserName(User);
            if (userIDSession == null)
            {
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                return RedirectToAction("Index", "Catalogo");
            }
            var items = from o in _context.DbSetPreOrden
                where o.UserName.Equals(userIDSession) && o.Status.Equals("PENDIENTE")
                select o;
            items = items.Include(p => p.Producto).OrderBy(o => o.Id);
            var itemsCarrito = items.ToList();

             // Obtener moneda seleccionada
            var selectedCurrency = HttpContext.Session.GetString("Currency") ?? "USD";
            decimal rate = 1;

            if (selectedCurrency != "USD")
            {
                rate = await _currencyService.GetExchangeRateAsync("USD", selectedCurrency);
            }

            // Convertir precios
            foreach (var item in itemsCarrito)
            {
                item.Precio = Math.Round(item.Precio * rate, 2);
            }

            var total = itemsCarrito.Sum(c => c.Cantidad * c.Precio);
            dynamic model = new ExpandoObject();
            model.montoTotal = total;
            model.elementosCarrito = itemsCarrito;
            // Almacenar el monto total en TempData para su uso posterior
            ViewBag.ClientId = "AadGUZ-ytOxd0qAzUW-ZbBYuIgz87ZOa341LxVz8bTJCL-R0OEGYv3eBxf-CnPDNC0lbFqIx8tIwhJV-";
            TempData["TotalAmount"] = total.ToString("F2");
            ViewBag.Currency = selectedCurrency;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}