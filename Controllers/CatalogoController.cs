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

namespace zapat.Controllers
{
    
    
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;

        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context)
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

        public async Task<IActionResult> Details(int? id)
        {
            // Aquí puedes agregar la lógica para obtener el producto por su ID
            // y pasarlo a la vista.

            Producto objProduct = await _context.DbSetProducto.FindAsync(id);
            if (objProduct == null)
            {
                return NotFound();
            }
            return View(objProduct);
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}