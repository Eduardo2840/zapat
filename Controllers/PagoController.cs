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

namespace zapat.Controllers
{
    
    public class PagoController : Controller
    {
        private readonly ILogger<PagoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PagoController(ILogger<PagoController> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Create(decimal monto)
        {

            Pago pago = new Pago();
            pago.UserName = _userManager.GetUserName(User);
            pago.MontoTotal = monto;
            _logger.LogInformation("El monto es: ${monto}", pago.MontoTotal.ToString());
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