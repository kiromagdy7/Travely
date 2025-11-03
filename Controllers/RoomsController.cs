using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Travely.Controllers
{

    [Authorize(Roles = "admin, staff")]
    public class RoomsController : Controller
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? hotelId)
        {
            var roomsQuery = _context.TblRooms.Include(r => r.Hotel).AsQueryable();
            if (hotelId.HasValue && hotelId.Value > 0)
            {
                roomsQuery = roomsQuery.Where(r => r.HotelId == hotelId.Value);

                return View(await roomsQuery.ToListAsync());
            }
            // Get the current user's role
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            // If user is admin, show all rooms
            if (userRole == "admin")
            {
                return View(await _context.TblRooms.Include(r => r.Hotel).ToListAsync());
            }
            
            // If user is staff, get their hotel ID
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var staffUser = await _context.TblUsers
                .FirstOrDefaultAsync(u => u.Email == userEmail);
                
            if (staffUser?.HotelId == null)
            {
                // If staff has no assigned hotel, show empty list
                //return View(new List<TblRoom>());
                return View(await roomsQuery.ToListAsync());
            }

            // Show only rooms for staff's hotel
            var hotelRooms = await _context.TblRooms
                .Include(r => r.Hotel)
                .Where(r => r.HotelId == staffUser.HotelId)
                .ToListAsync();
                
            return View(hotelRooms);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tblRoom = await _context.TblRooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (tblRoom == null) return NotFound();
            return View(tblRoom);
        }

        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HotelId,RoomNumber,RoomType,BedsCount,Price,MaxGuests,Description,BreakfastIncluded,PetsAllowed,Available")] TblRoom tblRoom)
        {
            if (ModelState.IsValid)
            {
                tblRoom.CreatedAt = DateTime.UtcNow;
                _context.Add(tblRoom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        private async Task<bool> CanManageRoom(int hotelId)
        {
            if (User.IsInRole("admin")) return true;
            
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var staffUser = await _context.TblUsers
                .FirstOrDefaultAsync(u => u.Email == userEmail);
                
            return staffUser?.HotelId == hotelId;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var tblRoom = await _context.TblRooms.FindAsync(id);
            if (tblRoom == null) return NotFound();
            
            // Check if staff can manage this room
            if (!await CanManageRoom(tblRoom.HotelId))
            {
                return Forbid();
            }
            
            // If admin or staff of this hotel
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,HotelId,RoomNumber,RoomType,BedsCount,Price,MaxGuests,Description,BreakfastIncluded,PetsAllowed,Available,CreatedAt")] TblRoom tblRoom)
        {
            if (id != tblRoom.RoomId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingRoom = await _context.TblRooms.AsNoTracking()
                        .FirstOrDefaultAsync(r => r.RoomId == id);

                    if (existingRoom == null)
                        return NotFound();

                    // Ensure HotelId isn't changed
                    tblRoom.HotelId = existingRoom.HotelId;

                    _context.Update(tblRoom);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblRoomExists(tblRoom.RoomId))
                        return NotFound();
                    throw;
                }
            }

            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tblRoom = await _context.TblRooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (tblRoom == null) return NotFound();
            return View(tblRoom);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblRoom = await _context.TblRooms.FindAsync(id);
            if (tblRoom != null)
            {
                _context.TblRooms.Remove(tblRoom);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateAvailability(int id, bool isAvailable)
        {
            var room = await _context.TblRooms.FindAsync(id);
            if (room == null) return NotFound();
            room.Available = isAvailable;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrice(int id, decimal newPrice)
        {
            if (newPrice <= 0) return BadRequest("Price must be greater than zero");
            var room = await _context.TblRooms.FindAsync(id);
            if (room == null) return NotFound();
            room.Price = newPrice;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableRooms(string? roomType = null)
        {
            var query = _context.TblRooms.Where(r => r.Available);
            if (!string.IsNullOrEmpty(roomType))
            {
                query = query.Where(r => r.RoomType == roomType);
            }
            var availableRooms = await query
                .Include(r => r.Hotel)
                .OrderBy(r => r.Price)
                .ToListAsync();
            return View("Index", availableRooms);
        }

        private bool TblRoomExists(int id)
        {
            return _context.TblRooms.Any(e => e.RoomId == id);
        }
    }
}