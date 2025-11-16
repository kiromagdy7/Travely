using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Travely.Data;
using Travely.Dtos.Bookings;
using Travely.Models;
using Travely.Services.Bookings;

namespace Travely.Controllers
{
    public class TblBookingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBookingService _bookingService;

        public TblBookingsController(AppDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.TblBookings.Include(t => t.Room);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tblBooking = await _context.TblBookings
                .Include(t => t.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (tblBooking == null) return NotFound();

            return View(tblBooking);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateRoomsDropDownAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,RoomId,CheckIn,CheckOut,Adults,Children")] TblBooking tblBooking)
        {
            if (tblBooking.CheckIn is null || tblBooking.CheckOut is null)
                ModelState.AddModelError("", "Check-in and Check-out dates are required.");

            if (ModelState.IsValid)
            {
                var dto = new CreateBookingDto
                {
                    UserId = tblBooking.UserId,
                    RoomId = tblBooking.RoomId,
                    CheckIn = tblBooking.CheckIn!.Value,
                    CheckOut = tblBooking.CheckOut!.Value,
                    Adults = tblBooking.Adults,
                    Children = tblBooking.Children
                };

                var result = await _bookingService.CreateAsync(dto);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
            }

            await PopulateRoomsDropDownAsync(tblBooking.RoomId);
            return View(tblBooking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tblBooking = await _context.TblBookings.FindAsync(id);
            if (tblBooking == null) return NotFound();

            await PopulateRoomsDropDownAsync(tblBooking.RoomId);
            return View(tblBooking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,UserId,RoomId,CheckIn,CheckOut,Adults,Children")] TblBooking tblBooking)
        {
            if (id != tblBooking.BookingId) return NotFound();

            if (tblBooking.CheckIn is null || tblBooking.CheckOut is null)
                ModelState.AddModelError("", "Check-in and Check-out dates are required.");

            if (ModelState.IsValid)
            {
                var dto = new UpdateBookingDto
                {
                    BookingId = tblBooking.BookingId,
                    UserId = tblBooking.UserId,
                    RoomId = tblBooking.RoomId,
                    CheckIn = tblBooking.CheckIn!.Value,
                    CheckOut = tblBooking.CheckOut!.Value,
                    Adults = tblBooking.Adults,
                    Children = tblBooking.Children
                };

                var result = await _bookingService.UpdateAsync(dto);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message);
            }

            await PopulateRoomsDropDownAsync(tblBooking.RoomId);
            return View(tblBooking);
        }

        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return NotFound();

            var tblBooking = await _context.TblBookings
                .Include(t => t.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (tblBooking == null) return NotFound();

            return View(tblBooking);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var result = await _bookingService.CancelAsync(id);

            if (!result.Success)
                TempData["ErrorMessage"] = result.Message;
            else
                TempData["SuccessMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateRoomsDropDownAsync(int? selectedRoomId = null)
        {
            var rooms = await _context.TblRooms
                .AsNoTracking()
                .Select(r => new
                {
                    r.RoomId,
                    Display = string.IsNullOrWhiteSpace(r.RoomNumber)
                        ? $"Room {r.RoomId}"
                        : r.RoomNumber
                })
                .ToListAsync();

            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "Display", selectedRoomId);
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int roomId)
        {
            var room = await _context.TblRooms
                .Include(r => r.TblRoomImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
                return NotFound();

            ViewBag.RoomPrice = room.Price;
            ViewBag.RoomType = room.RoomType;
            ViewBag.ImageUrl = room.TblRoomImages?.FirstOrDefault()?.ImageUrl
                               ?? "/images/no-image.jpg";

            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("nameidentifier");
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            var model = new CreateBookingDto
            {
                RoomId = roomId,
                UserId = userId
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(CreateBookingDto dto)
        {
            if (dto.CheckIn == null || dto.CheckOut == null)
            {
                ModelState.AddModelError("", "Please select check-in and check-out dates.");
                return View(dto);
            }

            if (dto.CheckIn >= dto.CheckOut)
            {
                ModelState.AddModelError("", "Check-out must be after the check-in date.");
                return View(dto);
            }

            var result = await _bookingService.CreateAsync(dto);

            if (!result.Success)
            {
                var room = await _context.TblRooms.AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RoomId == dto.RoomId);

                ViewBag.RoomPrice = room?.Price ?? 0m;
                ViewBag.ImageUrl = "/images/no-image.jpg";

                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            TempData["SuccessMessage"] = "Booking completed successfully!";
            return RedirectToAction("Details", new { id = result.Data!.BookingId });
        }
    }
}
