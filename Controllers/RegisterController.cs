using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using zapat.Data;
using zapat.Models;

namespace zapat.Controllers
{
    
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterController(ILogger<RegisterController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
    
        
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(Register register)
        {
            if (ModelState.IsValid)
            {
                try{
                    //simulate saving the contact information to a database
                    _context.DbSetRegisters.Add(register);
                    _context.SaveChanges();
                    _logger.LogInformation("Se registro");
                    ViewData["Message"]="Se registro";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al registrar");
                    ViewData["Message"] = "Error al registrar" + ex.Message;
                }
            }
            else
            {
                ViewData["Message"] = "Datos de entrada no validos";
            }
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}