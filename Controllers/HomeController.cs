using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Travely.Models;
using Microsoft.AspNetCore.Authorization; // <-- 1. Add this using statement

namespace Travely.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous] // <-- 2. Add this attribute
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous] // <-- 3. Add this attribute
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous] // <-- 4. Add this attribute
        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error(int? statusCode = null)
        {
            var error = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode,
                ErrorMessage = statusCode switch
                {
                    404 => "The page you're looking for doesn't exist.",
                    500 => "An internal server error occurred.",
                    _ => "An error occurred while processing your request."
                }
            };

            return View(error);
        }
    }
}