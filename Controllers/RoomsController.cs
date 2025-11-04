using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System;

namespace Travely.Controllers
{

    [Authorize(Roles = "admin, staff")]
    public class RoomsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
                .Include(r => r.TblRoomImages) // <-- هذا السطر موجود وهو صحيح
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (tblRoom == null) return NotFound();
            return View(tblRoom);
        }

        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name");
            return View(new TblRoom());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("HotelId,RoomNumber,RoomType,BedsCount,Price,MaxGuests,Description,BreakfastIncluded,PetsAllowed,Available")] TblRoom tblRoom,
            List<IFormFile> images)
        {
            // (كود التحقق من الحجم)
            long maxFileSize = 5 * 1024 * 1024; // 5 MB
            foreach (var file in images)
            {
                if (file.Length > maxFileSize)
                {
                    ModelState.AddModelError(string.Empty, $"لا يصلح ان تكون الصورة '{file.FileName}' أكثر من 5 ميجا.");
                }
            }

            if (ModelState.IsValid)
            {
                tblRoom.CreatedAt = DateTime.UtcNow;
                _context.Add(tblRoom);
                await _context.SaveChangesAsync(); // <-- الحفظ الأول

                if (images != null && images.Count > 0)
                {
                    // (كود حفظ الصور)
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms");
                    Directory.CreateDirectory(uploadFolder);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    int imageCounter = 1;

                    foreach (var file in images)
                    {
                        string extension = Path.GetExtension(file.FileName);
                        string uniqueFileName = $"{timestamp}_{tblRoom.RoomId}_{imageCounter++}{extension}";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        var roomImage = new TblRoomImage
                        {
                            RoomId = tblRoom.RoomId,
                            ImageUrl = "/images/rooms/" + uniqueFileName
                        };
                        _context.TblRoomImages.Add(roomImage);
                    }
                    await _context.SaveChangesAsync(); // <-- الحفظ الثاني
                }
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

        // ---------- [ بداية التعديلات المطلوبة ] ----------

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // <-- 1. تعديل: استخدام Include لجلب الصور الموجودة مع الغرفة
            var tblRoom = await _context.TblRooms
                .Include(r => r.TblRoomImages)
                .FirstOrDefaultAsync(r => r.RoomId == id);

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
        // <-- 2. تعديل: إضافة باراميتر "List<IFormFile> images"
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,HotelId,RoomNumber,RoomType,BedsCount,Price,MaxGuests,Description,BreakfastIncluded,PetsAllowed,Available,CreatedAt")] TblRoom tblRoom, List<IFormFile> images)
        {
            if (id != tblRoom.RoomId) return NotFound();

            // <-- 3. إضافة: التحقق من حجم الصور الجديدة (5 ميجا)
            long maxFileSize = 5 * 1024 * 1024; // 5 MB
            foreach (var file in images)
            {
                if (file.Length > maxFileSize)
                {
                    ModelState.AddModelError(string.Empty, $"لا يصلح ان تكون الصورة '{file.FileName}' أكثر من 5 ميجا.");
                }
            }

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

                    // <-- 4. إضافة: منطق حفظ الصور الجديدة (نفس كود Create)
                    if (images != null && images.Count > 0)
                    {
                        string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "rooms");
                        Directory.CreateDirectory(uploadFolder);
                        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        int imageCounter = 1;

                        foreach (var file in images)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            string uniqueFileName = $"{timestamp}_{tblRoom.RoomId}_{imageCounter++}{extension}";
                            string filePath = Path.Combine(uploadFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                            var roomImage = new TblRoomImage
                            {
                                RoomId = tblRoom.RoomId,
                                ImageUrl = "/images/rooms/" + uniqueFileName
                            };
                            _context.TblRoomImages.Add(roomImage); // إضافة الصور الجديدة
                        }
                    }

                    _context.Update(tblRoom); // تحديث بيانات الغرفة
                    await _context.SaveChangesAsync(); // حفظ التحديثات والصور الجديدة معاً
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblRoomExists(tblRoom.RoomId))
                        return NotFound();
                    throw;
                }
            }

            // <-- 5. تعديل: إذا فشل الحفظ، أعد تحميل الصور الموجودة
            tblRoom.TblRoomImages = await _context.TblRoomImages
                                        .Where(i => i.RoomId == id).ToListAsync();
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        // <-- 6. إضافة: دالة جديدة لحذف الصور
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRoomImage(int imageId, int roomId)
        {
            // البحث عن سجل الصورة
            var image = await _context.TblRoomImages.FindAsync(imageId);
            if (image == null)
            {
                TempData["ErrorMessage"] = "Image not found.";
                return RedirectToAction(nameof(Edit), new { id = roomId });
            }

            // التأكد من الصلاحية
            var room = await _context.TblRooms.FindAsync(roomId);
            if (room == null || !await CanManageRoom(room.HotelId))
            {
                return Forbid();
            }

            try
            {
                // 1. حذف الملف الفعلي من المجلد
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // 2. حذف سجل الصورة من قاعدة البيانات
                _context.TblRoomImages.Remove(image);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Image removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error removing image: " + ex.Message;
            }

            // 3. العودة لصفحة التعديل
            return RedirectToAction(nameof(Edit), new { id = roomId });
        }


        // ---------- [ نهاية التعديلات المطلوبة ] ----------

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
            // <-- 7. تعديل: جلب الصور مع الغرفة لحذفها
            var tblRoom = await _context.TblRooms
                .Include(r => r.TblRoomImages)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (tblRoom != null)
            {
                // 8. إضافة: حذف الملفات الفعلية للصور
                foreach (var image in tblRoom.TblRoomImages)
                {
                    try
                    {
                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch (Exception)
                    {
                        // (يمكنك تسجيل الخطأ هنا إذا فشل حذف ملف)
                    }
                }

                // (سيقوم EF Core بحذف سجلات TblRoomImages المرتبطة عند حذف الغرفة بسبب العلاقة)
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