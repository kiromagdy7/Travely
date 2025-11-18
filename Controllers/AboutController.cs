using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travely.Data; // Kiro: Add this
using Travely.Models; // Kiro: Add this
using Microsoft.EntityFrameworkCore; // Kiro: Add this
using System.Threading.Tasks; // Kiro: Add this
using System.Linq; // Kiro: Add this
using System.Collections.Generic; // Kiro: Add this

namespace Travely.Controllers
{
    [AllowAnonymous]
    public class AboutController : Controller
    {
        private readonly AppDbContext _context; // Kiro: Add AppDbContext

        // Kiro: Update constructor
        public AboutController(AppDbContext context)
        {
            _context = context;
        }

        // Kiro: Update Index to be async and fetch data
        public async Task<IActionResult> Index()
        {
            // Kiro: Get 4 rooms to feature (e.g., top 4 most expensive)
            var featuredRooms = await _context.TblRooms
                .Include(r => r.TblRoomImages)
                .OrderByDescending(r => r.Price)
                .Take(4)
                .ToListAsync();

            return View(featuredRooms); // Kiro: Pass the list of rooms to the View
        }
    }
}