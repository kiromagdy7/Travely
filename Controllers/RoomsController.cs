    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Travely.Data;
using Travely.Models;
using System.Threading.Tasks;
using System.Linq;

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
            ViewData["HotelId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TblHotel, "Id", "Name");
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,HotelId,RoomType,RoomNumber,Price,Available,Description,Capacity")] TblRoom tblRoom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblRoom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TblHotel, "Id", "Name", tblRoom.HotelId);
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
            ViewData["HotelId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TblHotel, "Id", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,HotelId,RoomType,RoomNumber,Price,IsAvailable,Description,Capacity")] TblRoom tblRoom)
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
            ViewData["HotelId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.TblHotels, "HotelId", "HotelName", tblRoom.HotelId);
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
                .Include(r => r.TblHotel)
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

        // Methods for handling room availability and pricing
        
        // GET: Rooms/UpdateAvailability/5
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

        // GET: Rooms/UpdatePrice/5
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

        // GET: Rooms/GetAvailableRooms
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
