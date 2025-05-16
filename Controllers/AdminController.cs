using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using zapat.Data;
using zapat.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace zapat.Controllers
{

    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var pedidos = from o in _context.DbSetAdmin select o;
            pedidos = pedidos.Include(p => p.Pago).Where(s => s.Status.Contains("PENDIENTE"));
            return View(await pedidos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            var itemsAdmin = from o in _context.DbSetDetalleOrden select o;

            itemsAdmin = itemsAdmin.
                Include(p => p.Producto).
                Include(p => p.Admin).
                Where(s => s.Id.Equals(id));

            var itemsList = await itemsAdmin.ToListAsync();
            var itemsJson = JsonSerializer.Serialize(itemsList);
            _logger.LogInformation("Items en JSON: {json}", itemsJson);


            return View(itemsAdmin);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}