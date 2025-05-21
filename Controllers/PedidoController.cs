using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using zapat.Data;

namespace zapat.Controllers
{
   
    public class PedidoController : Controller
    {
        private readonly ILogger<PedidoController> _logger;
        private readonly ApplicationDbContext _context;

        public PedidoController(ILogger<PedidoController> logger, ApplicationDbContext context)
        {
            _logger = logger;       
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = from o in _context.DbSetOrden select o;
            pedidos = pedidos.Include(p => p.Pago).Where(s => s.Status.Contains("PENDIENTE"));
            return View(await pedidos.ToListAsync());
        }
         public async Task<IActionResult> Details(int? id)
        {
            var itemsPedido = from o in _context.DbSetDetalleOrden select o;

            itemsPedido = itemsPedido.
                Include(p => p.Producto).
                Include(p => p.Orden).
                Where(s => s.Id.Equals(id));

            var itemsList = await itemsPedido.ToListAsync();
            var itemsJson = JsonSerializer.Serialize(itemsList);
            _logger.LogInformation("Items en JSON: {json}", itemsJson);


            return View(itemsList);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}