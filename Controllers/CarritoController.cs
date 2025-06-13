using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using zapat.Data;
using zapat.Models;
using zapat.Services;

namespace zapat.Controllers
{

    public class CarritoController : Controller
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarritoController(ILogger<CatalogoController> logger, ApplicationDbContext context, CurrencyService currencyService, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> Index()
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

            items = items.Include(p => p.Producto).OrderBy(o => o.Id); // Asegúrate de que estén ordenados por Id o alguna propiedad consistente.
            var itemsCarrito = items.ToList();

            var selectedCurrency = HttpContext.Session.GetString("Currency") ?? "USD";
            decimal rate = 1;
            if (selectedCurrency != "USD")
            {
                rate = await _currencyService.GetExchangeRateAsync("USD", selectedCurrency);
            }

            // === AGREGADO: Aplicar tasa a precios ===
            foreach (var item in itemsCarrito)
            {
                item.Precio = Math.Round(item.Precio * rate, 2);
            }

            var total = itemsCarrito.Sum(c => c.Cantidad * c.Precio);

            dynamic model = new ExpandoObject();
            model.montoTotal = total;
            model.elementosCarrito = itemsCarrito;
            // Almacenar el monto total en TempData para su uso posterior
            TempData["TotalAmount"] = total.ToString("F2");
            ViewBag.Currency = selectedCurrency;
            return View(model);
        }


        [Authorize]
        public async Task<IActionResult> Add(int? id)
        {
            var userID = _userManager.GetUserName(User);
            if (userID == null)
            {
                _logger.LogInformation("No existe usuario");
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                return RedirectToAction("Index", "Catalogo");
            }
            else
            {
                var producto = await _context.DbSetProducto.FindAsync(id);
                PreOrden proforma = new PreOrden();

                proforma.Producto = producto;
                proforma.Precio = producto.Price;
                proforma.Cantidad = 1;
                proforma.UserName = userID;
                _context.Add(proforma);
                await _context.SaveChangesAsync();
                ViewData["Message"] = "Se Agrego al carrito";
                _logger.LogInformation("Se agrego un producto al carrito");
                return RedirectToAction("Index", "Catalogo");
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var preorden = await _context.DbSetPreOrden.FindAsync(id);
            if (preorden != null)
            {
                _context.DbSetPreOrden.Remove(preorden);
                await _context.SaveChangesAsync();
                ViewData["Message"] = "Se elimino del carrito";
                _logger.LogInformation("Se elimino un producto del carrito");
            }
            return RedirectToAction("Index", "Carrito");
        }

        [HttpPost]
        public async Task<IActionResult> Incrementar(int id)
        {
            var item = await _context.DbSetPreOrden.FindAsync(id);
            if (item != null)
            {
                item.Cantidad++;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Decrementar(int id)
        {
            var item = await _context.DbSetPreOrden.FindAsync(id);
            if (item != null && item.Cantidad > 1)
            {
                item.Cantidad--;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}