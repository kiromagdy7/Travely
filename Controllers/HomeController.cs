using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Travely.Models;
using Microsoft.AspNetCore.Authorization;
using Travely.Data; // Kiro: Add this to access AppDbContext
using Travely.ViewModels; // Kiro: Add this to access HomeViewModel
using Microsoft.EntityFrameworkCore; // Kiro: Add this for .Include() and .ToListAsync()

namespace Travely.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; // Kiro: Add AppDbContext

        // Kiro: Update constructor to get AppDbContext
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Kiro: Get 3 hotels to feature (e.g., in the "Offers" section)
            var hotels = await _context.TblHotels
                .Include(h => h.TblHotelImages) // Get images
                .OrderByDescending(h => h.Stars) // Get top rated
                .Take(3)
                .ToListAsync();

            // Kiro: Get 4 rooms to feature (e.g., in the "Rooms Showcase" section)
            var rooms = await _context.TblRooms
                .Include(r => r.TblRoomImages) // Get images
                .OrderBy(r => r.Price) // Get cheapest
                .Take(4)
                .ToListAsync();

            // Kiro: Create the ViewModel
            var viewModel = new HomeViewModel
            {
                FeaturedHotels = hotels,
                FeaturedRooms = rooms
            };

            return View(viewModel); // Kiro: Pass the ViewModel to the View
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
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