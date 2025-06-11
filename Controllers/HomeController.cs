using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using zapat.Models;
using Microsoft.Extensions.Logging;
using zapat.Data;

namespace zapat.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
         private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.DbSetProducto.ToList();
            _logger.LogInformation("Productos: {0}", productos);
            return View(productos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
}
