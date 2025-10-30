using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;

namespace Travely.Controllers
{
    // هنقفل الكنترولر ده للكاستمر بس
    [Authorize(Roles = "customer")]
    public class WishListController : Controller
    {
        private readonly AppDbContext _context;

        public WishListController(AppDbContext context)
        {
            _context = context;
        }

        // عشان نجيب الـ ID بتاع اليوزر اللي مسجل دخول
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        // GET: WishList (صفحة قايمة أمنياتي)
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();

            // هنجيب قايمة الأمنيات بتاعة اليوزر ده بس
            // وهنعمل Include للفندق عشان نعرض بياناته (وده سبب الخطوة 1)
            var wishlist = await _context.TblWishLists
                .Where(w => w.UserId == currentUserId)
                .Include(w => w.Hotel) // <-- عشان كده صلحنا الموديل
                .ToListAsync();

            return View(wishlist);
        }

        // POST: WishList/Add/5 (رقم 5 هو id الفندق)
        [HttpPost]
        public async Task<IActionResult> Add(int hotelId)
        {
            var currentUserId = GetCurrentUserId();

            // 1. اتأكد إن الفندق موجود أصلاً
            var hotelExists = await _context.TblHotels.AnyAsync(h => h.HotelId == hotelId);
            if (!hotelExists)
            {
                return NotFound("Hotel not found.");
            }

            // 2. اتأكد إن اليوزر مضفش الفندق ده قبل كده
            var alreadyInWishlist = await _context.TblWishLists
                .AnyAsync(w => w.UserId == currentUserId && w.HotelId == hotelId);

            if (alreadyInWishlist)
            {
                // ممكن ترجع رسالة إنه موجود قبل كده
                return RedirectToAction("Index", "Hotels"); // أو أي صفحة تانية
            }

            // 3. ضيف الفندق
            var wishListItem = new TblWishList
            {
                UserId = currentUserId,
                HotelId = hotelId,
                AddedDate = DateTime.UtcNow
            };

            _context.TblWishLists.Add(wishListItem);
            await _context.SaveChangesAsync();

            // رجعه لقايمة أمنياته
            return RedirectToAction(nameof(Index));
        }


        // POST: WishList/Remove/10 (رقم 10 هو id بتاع الأمنية نفسها)
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var currentUserId = GetCurrentUserId();

            // 1. دور على الأمنية
            var wishListItem = await _context.TblWishLists
                .FirstOrDefaultAsync(w => w.WishlistId == id);

            // 2. اتأكد إنها موجودة، وإنها "بتاعة" اليوزر ده (عشان ميقدرش يمسح أمنيات حد تاني)
            if (wishListItem == null)
            {
                return NotFound();
            }

            if (wishListItem.UserId != currentUserId)
            {
                // Access Denied!
                return Forbid();
            }

            // 3. امسحها
            _context.TblWishLists.Remove(wishListItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}