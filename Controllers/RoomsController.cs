using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering; // <-- Add this using statement

namespace Travely.Controllers
{
    public class RoomsController : Controller
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            // Use Hotel.Name which exists, not Hotel.HotelName
            return View(await _context.TblRooms.Include(r => r.Hotel).ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRoom = await _context.TblRooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (tblRoom == null)
            {
                return NotFound();
            }

            return View(tblRoom);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            // Corrected SelectList: Use "HotelId" and "Name" which are likely the correct properties
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name");
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Corrected Bind: Replaced "Capacity" with "MaxGuests" and all other existing properties
        public async Task<IActionResult> Create([Bind("HotelId,RoomNumber,RoomType,BedsCount,Price,MaxGuests,Description,BreakfastIncluded,PetsAllowed,Available")] TblRoom tblRoom)
        {
            if (ModelState.IsValid)
            {
                tblRoom.CreatedAt = DateTime.UtcNow; // Set creation date on server
                _context.Add(tblRoom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRoom = await _context.TblRooms.FindAsync(id);
            if (tblRoom == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Corrected Bind: Replaced "IsAvailable" with "Available" and "Capacity" with "MaxGuests"
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,HotelId,RoomNumber,RoomType,BedsCount,Price,MaxGuests,Description,BreakfastIncluded,PetsAllowed,Available,CreatedAt")] TblRoom tblRoom)
        {
            if (id != tblRoom.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblRoom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblRoomExists(tblRoom.RoomId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRoom = await _context.TblRooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (tblRoom == null)
            {
                return NotFound();
            }

            return View(tblRoom);
        }

        // POST: Rooms/Delete/5
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

        // --- Your other methods (UpdateAvailability, etc.) are fine ---

        public async Task<IActionResult> UpdateAvailability(int id, bool isAvailable)
        {
            var room = await _context.TblRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.Available = isAvailable;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrice(int id, decimal newPrice)
        {
            if (newPrice <= 0)
            {
                return BadRequest("Price must be greater than zero");
            }

            var room = await _context.TblRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.Price = newPrice;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetAvailableRooms(string roomType = null)
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