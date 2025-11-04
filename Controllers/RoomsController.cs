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

        // ---------- [ بداية التعديل المطلوب ] ----------
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? hotelId)
        {
            // 1. إضافة Include للصور هنا في الاستعلام الأساسي
            var roomsQuery = _context.TblRooms
                .Include(r => r.Hotel)
                .Include(r => r.TblRoomImages) // <--- الإضافة الأولى
                .AsQueryable();

            if (hotelId.HasValue && hotelId.Value > 0)
            {
                roomsQuery = roomsQuery.Where(r => r.HotelId == hotelId.Value);
                // البيانات ستأتي مع الصور لأنها موجودة في roomsQuery
                return View(await roomsQuery.ToListAsync());
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "admin")
            {
                // 2. إضافة Include للصور هنا (مسار الأدمن)
                return View(await _context.TblRooms
                    .Include(r => r.Hotel)
                    .Include(r => r.TblRoomImages) // <--- الإضافة الثانية
                    .ToListAsync());
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var staffUser = await _context.TblUsers
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (staffUser?.HotelId == null)
            {
                // سيعود بالبيانات من roomsQuery (الذي يحتوي على الصور)
                return View(await roomsQuery.ToListAsync());
            }

            // 3. إضافة Include للصور هنا (مسار الموظف)
            var hotelRooms = await _context.TblRooms
                .Include(r => r.Hotel)
                .Include(r => r.TblRoomImages) // <--- الإضافة الثالثة
                .Where(r => r.HotelId == staffUser.HotelId)
                .ToListAsync();

            return View(hotelRooms);
        }
        // ---------- [ نهاية التعديل المطلوب ] ----------


        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tblRoom = await _context.TblRooms
                .Include(r => r.Hotel)
                .Include(r => r.TblRoomImages) // (موجودة لديك)
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
                await _context.SaveChangesAsync();

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
                        _context.TblRoomImages.Add(roomImage);
                    }
                    await _context.SaveChangesAsync();
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tblRoom = await _context.TblRooms
                .Include(r => r.TblRoomImages) // (موجودة لديك)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (tblRoom == null) return NotFound();

            if (!await CanManageRoom(tblRoom.HotelId))
            {
                return Forbid();
            }

            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,HotelId,RoomNumber,RoomType,BedsCount,Price,MaxGuests,Description,BreakfastIncluded,PetsAllowed,Available,CreatedAt")] TblRoom tblRoom, List<IFormFile> images)
        {
            if (id != tblRoom.RoomId) return NotFound();

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

                    tblRoom.HotelId = existingRoom.HotelId;

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
                            _context.TblRoomImages.Add(roomImage);
                        }
                    }

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

            tblRoom.TblRoomImages = await _context.TblRoomImages
                                        .Where(i => i.RoomId == id).ToListAsync();
            ViewData["HotelId"] = new SelectList(_context.TblHotels, "HotelId", "Name", tblRoom.HotelId);
            return View(tblRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRoomImage(int imageId, int roomId)
        {
            var image = await _context.TblRoomImages.FindAsync(imageId);
            if (image == null)
            {
                TempData["ErrorMessage"] = "Image not found.";
                return RedirectToAction(nameof(Edit), new { id = roomId });
            }

            var room = await _context.TblRooms.FindAsync(roomId);
            if (room == null || !await CanManageRoom(room.HotelId))
            {
                return Forbid();
            }

            try
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _context.TblRoomImages.Remove(image);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Image removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error removing image: " + ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = roomId });
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
            var tblRoom = await _context.TblRooms
                .Include(r => r.TblRoomImages) // (موجودة لديك)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (tblRoom != null)
            {
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
                        // Log error if deleting file fails
                    }
                }

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

                // .Include(r => r.TblRoomImages) 
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