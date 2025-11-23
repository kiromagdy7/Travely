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

        // ---------------------------------------------------------
        // NEW METHODS ADDED BELOW FOR FRONT-END BOOKING FLOW
        // ---------------------------------------------------------
        // ... داخل كلاس TblBookingsController
        // ... داخل الكلاس TblBookingsController

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            // 1. الحصول على رقم المستخدم الحالي
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim.Value);

            // 2. جلب الحجوزات الخاصة بهذا المستخدم فقط
            var myBookings = await _context.TblBookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                // مهم: نضمن وجود الصور عشان مايحصلش Null Reference في العرض
                .Include(b => b.Room.TblRoomImages)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return View(myBookings);
        }
        [Authorize] // لازم يكون مسجل دخول

        [Authorize]
        public async Task<IActionResult> Confirm(int roomId)
        {
            var room = await _context.TblRooms
                .Include(r => r.TblRoomImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
                return NotFound();

            // Populate ViewBag for the initial page load
            ViewBag.RoomId = room.RoomId;
            ViewBag.RoomPrice = room.Price;
            ViewBag.RoomType = room.RoomType;
            ViewBag.ImageUrl = room.TblRoomImages?.FirstOrDefault()?.ImageUrl
                               ?? "/images/rooms/unknown_room.jpg";

            // Get current user ID
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
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
            // 1. تصحيح UserId يدوياً من السيرفر (أكثر أماناً وضماناً)
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                dto.UserId = int.Parse(userIdClaim.Value);
                // مسح أي خطأ خاص بالـ UserId من الـ Validation لأنه أصبح صحيحاً الآن
                ModelState.Remove("UserId");
            }

            // 2. التحقق من التواريخ
            if (dto.CheckIn == default || dto.CheckOut == default)
            {
                ModelState.AddModelError("", "Please select valid check-in and check-out dates.");
            }
            else if (dto.CheckIn >= dto.CheckOut)
            {
                ModelState.AddModelError("", "Check-out must be after the check-in date.");
            }

            // 3. محاولة الحجز
            if (ModelState.IsValid)
            {
                var result = await _bookingService.CreateAsync(dto);

                if (result.Success)
                {
                    return RedirectToAction("CreateCheckoutSession", "Payment", new { bookingId = result.Data!.BookingId });
                }

                // لو الخدمة رجعت خطأ (مثلاً الغرفة محجوزة في الوقت ده)
                ModelState.AddModelError("", result.Message);
            }

            // ============================================================
            // 4. في حالة الفشل: إعادة تحميل بيانات الغرفة عشان الصفحة ماتفضاش
            // ============================================================
            var room = await _context.TblRooms
                .Include(r => r.TblRoomImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomId == dto.RoomId);

            if (room != null)
            {
                ViewBag.RoomId = room.RoomId;
                ViewBag.RoomPrice = room.Price;
                ViewBag.RoomType = room.RoomType;
                ViewBag.ImageUrl = room.TblRoomImages?.FirstOrDefault()?.ImageUrl ?? "/images/rooms/unknown_room.jpg";
            }
            else
            {
                ViewBag.ImageUrl = "/images/rooms/unknown_room.jpg";
            }

            // رجوع للصفحة مع عرض الأخطاء
            return View(dto);
        }
    }
}