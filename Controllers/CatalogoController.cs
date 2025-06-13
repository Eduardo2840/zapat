using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using zapat.Data;
using zapat.Models;
using Microsoft.AspNetCore.Authorization;
using zapat.Services;

namespace zapat.Controllers
{
    
    
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context, CurrencyService currencyService, IHttpContextAccessor httpContextAccessor)
{
            _logger = logger;
            _context = context;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var productos = _context.DbSetProducto.ToList();
            var selectedCurrency = HttpContext.Session.GetString("Currency") ?? "USD";

            if (selectedCurrency != "USD") // Suponemos que los precios base están en USD
            {
                var rate = await _currencyService.GetExchangeRateAsync("USD", selectedCurrency);
                foreach (var p in productos)
                {
                    p.Price = Math.Round(p.Price * rate, 2); // Convertimos el precio
                }
            }

            ViewBag.Currency = selectedCurrency; // Enviamos a la vista la divisa actual

            return View(productos);
        }
        
        [HttpPost]
        public IActionResult ChangeCurrency(string currency)
        {
            HttpContext.Session.SetString("Currency", currency);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            // Aquí puedes agregar la lógica para obtener el producto por su ID
            // y pasarlo a la vista.

            if (id == null)
            {
                return NotFound();
            }

            Producto objProduct = await _context.DbSetProducto.FindAsync(id);
            if (objProduct == null)
            {
                return NotFound();
            }

            var selectedCurrency = HttpContext.Session.GetString("Currency") ?? "USD";
            if (selectedCurrency != "USD")
            {
                var rate = await _currencyService.GetExchangeRateAsync("USD", selectedCurrency);
                objProduct.Price = Math.Round(objProduct.Price * rate, 2);
            }

            ViewBag.Currency = selectedCurrency;

            return View(objProduct);

        }

         public async Task<IActionResult> Hombre(int? id)
        {
            var productos = _context.DbSetProHombre.ToList();
            _logger.LogInformation("Productos: {0}", productos);
            return View(productos);
        }
        
         public async Task<IActionResult> Mujer(int? id)
        {
            var productos = _context.DbSetProMujer.ToList();
            _logger.LogInformation("Productos: {0}", productos);
            return View(productos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}