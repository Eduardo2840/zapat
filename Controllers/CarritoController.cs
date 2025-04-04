using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace zapat.Controllers
{
    public class CarritoController : Controller
    {
       // Lista estática para simular un carrito temporal
        private static List<string> carrito = new List<string>();

        public IActionResult Index()
        {
            // Pasar los productos al Index de Carrito
            return View(carrito);
        }

        [HttpPost]
        public IActionResult AgregarProducto(string producto)
        {
            if (!string.IsNullOrEmpty(producto))
            {
                carrito.Add(producto); // Agregar el producto al carrito
                return Json(new { success = true, message = "Producto agregado al carrito" });
            }
            return Json(new { success = false, message = "El producto no puede estar vacío" });
        }
        
        
    }
}