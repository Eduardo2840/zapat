using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using zapat.Models;
using Microsoft.Extensions.Logging;
using zapat.Data;
using zapat.Services;

namespace zapat.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
         private readonly ApplicationDbContext _context;
         private readonly CurrencyService _currencyService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, CurrencyService currencyService)
    {
        _logger = logger;
        _context = context;
        _currencyService = currencyService;
    }

        public async Task<IActionResult> Index()
        {
            var productos = _context.DbSetProducto.ToList();
            var selectedCurrency = HttpContext.Session.GetString("Currency") ?? "USD";

            if (selectedCurrency != "USD")
            {
                var rate = await _currencyService.GetExchangeRateAsync("USD", selectedCurrency);
                foreach (var p in productos)
                {
                    p.Price = Math.Round(p.Price * rate, 2);
                }
            }

            ViewBag.Currency = selectedCurrency;
            return View(productos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
}
