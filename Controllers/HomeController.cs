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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] // <-- 4. Add this attribute
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}