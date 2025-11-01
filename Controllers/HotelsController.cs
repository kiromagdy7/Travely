using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Dtos.Hotels;
using Travely.Models;
using Travely.Services.Hotels;
using Microsoft.AspNetCore.Http;

namespace Travely.Controllers
{
    [Authorize(Roles = "admin, staff")]
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly AppDbContext _context;

        public HotelsController(IHotelService hotelService, AppDbContext context)
        {
            _hotelService = hotelService;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var hotels = await _context.TblHotels.Include(h => h.TblHotelImages).ToListAsync();

            return View(hotels);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            
            var hotel = await _context.TblHotels.Include(h => h.TblHotelImages).FirstOrDefaultAsync(h => h.HotelId == id.Value);

            if (hotel == null) return NotFound();

            return View(hotel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateHotelDto dto, [FromForm] IFormFile[]? images)
        {
            if (!ModelState.IsValid) return View(dto);

            var (ok, message, hotelId) = await _hotelService.CreateAsync(dto, images);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var hotel = await _context.TblHotels.AsNoTracking().FirstOrDefaultAsync(h => h.HotelId == id.Value);
            if (hotel is null) return NotFound();

            var vm = new UpdateHotelDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Location = hotel.Location,
                Address = hotel.Address,
                Stars = hotel.Stars,
                Phone = hotel.Phone,
                ContactInfo = hotel.ContactInfo,
                CheckInTime = hotel.CheckInTime,
                CheckOutTime = hotel.CheckOutTime,
                CancellationPolicy = hotel.CancellationPolicy,
                Fees = hotel.Fees,
                Commission = hotel.Commission
            };

            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdateHotelDto dto, [FromForm] IFormFile[]? images)
        {
            if (id != dto.HotelId) return NotFound();
            if (!ModelState.IsValid) return View(dto);

            var (ok, message) = await _hotelService.UpdateAsync(dto, images);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var hotel = await _context.TblHotels.FirstOrDefaultAsync(h => h.HotelId == id.Value);
            if (hotel is null) return NotFound();

            return View(hotel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (ok, message) = await _hotelService.DeleteAsync(id);
            if (!ok)
            {
                TempData["ErrorMessage"] = message;
            }
            else
            {
                TempData["SuccessMessage"] = message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImage(int imageId, int hotelId)
        {
            var (ok, message) = await _hotelService.RemoveImageAsync(imageId);
            if (!ok) TempData["ErrorMessage"] = message;
            else TempData["SuccessMessage"] = message;

            return RedirectToAction(nameof(Edit), new { id = hotelId });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Rooms(int id)
        {
            var hotel = await _context.TblHotels
                .Include(h => h.TblRooms)
                .FirstOrDefaultAsync(h => h.HotelId == id);

            if (hotel is null) return NotFound();

            return View(hotel.TblRooms.ToList());
        }

        private bool TblHotelExists(int id) => _context.TblHotels.Any(e => e.HotelId == id);
    }
}