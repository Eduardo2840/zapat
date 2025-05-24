using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using zapat.Client;
using zapat.Data;
using zapat.Helper;

namespace zapat.Controllers
{
   
    public class CheckoutController : Controller
{
    public string TotalAmount { get; set; } = null;
    private readonly PayPalClient _paypalClient;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CheckoutController(PayPalClient paypalClient, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _paypalClient = paypalClient;
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var userID = _userManager.GetUserName(User);
        if (userID == null)
        {
            ViewData["Message"] = "Debe iniciar sesión para continuar con el checkout.";
            return RedirectToAction("Index", "Catalogo");
        }

        // Obtener elementos del carrito desde la base de datos (tabla PreOrden)
        var itemsCarrito = _context.DbSetPreOrden
            .Include(p => p.Producto)
            .Where(o => o.UserName == userID && o.Status == "PENDIENTE")
            .OrderBy(o => o.Id)
            .ToList();

        var total = itemsCarrito.Sum(item => item.Cantidad * item.Precio);

        ViewBag.ClientId = _paypalClient.ClientId;
        ViewBag.cart = itemsCarrito;
        ViewBag.DollarAmount = total;
        ViewBag.total = total;

        TotalAmount = total.ToString("F2");
        TempData["TotalAmount"] = TotalAmount;

        return View();
    }

    public IActionResult Processing(string stripeToken,string stripeEmail)
        {
    
            return View();


        }

    

        [HttpPost]
        public async Task<IActionResult> Order(CancellationToken cancellationToken)
        {
            
            try
            {
                // set the transaction price and currency
                var userID = _userManager.GetUserName(User);
                if (userID == null)
                {
                    return Unauthorized("Debe iniciar sesión");
                }

                // Obtener el total del carrito del usuario desde la base de datos
                var itemsCarrito = _context.DbSetPreOrden
                    .Where(o => o.UserName == userID && o.Status == "PENDIENTE")
                    .ToList();

                var total = itemsCarrito.Sum(item => item.Cantidad * item.Precio);

                if (total == 0)
                {
                    return BadRequest(new { Message = "El carrito está vacío" });
                }

                var price = total.ToString("F2", CultureInfo.InvariantCulture);
                var currency = "USD";

                // Crear un número de referencia único
                var reference = GetRandomInvoiceNumber();

                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return Json(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        public async Task<IActionResult> Capture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = response.purchase_units[0].reference_id;

                // Put your logic to save the transaction here
                // You can use the "reference" variable as a transaction key

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

    

        public static string GetRandomInvoiceNumber()
        {
            return new Random().Next(999999).ToString();
        }
        public IActionResult Success()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}
}