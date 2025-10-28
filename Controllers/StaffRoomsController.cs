using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;

namespace Travely.Controllers
{
    [Authorize(Roles = "staff")]
    public class StaffRoomsController : Controller
    {
        private readonly AppDbContext _context;

        public StaffRoomsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // ✅ Use TblUsers — not Users
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return Forbid();

            var staff = _context.TblUsers
                .Include(u => u.Hotel)
                .FirstOrDefault(u => u.Email == userEmail && u.Role == "Staff");

            if (staff == null || staff.HotelId == null)
                return Forbid();

            // ✅ Use TblRooms — not Rooms
            var rooms = _context.TblRooms
                .Include(r => r.Hotel)
                .Where(r => r.HotelId == staff.HotelId)
                .ToList();

            return View(rooms);
        }
    }
}
