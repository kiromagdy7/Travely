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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System;

namespace Travely.Controllers
{
    [Authorize(Roles = "admin, staff")]
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HotelsController(IHotelService hotelService, AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _hotelService = hotelService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create([FromForm] CreateHotelDto dto, [FromForm] IFormFile[]? ImageFiles)
        {
            if (!ModelState.IsValid) return View(dto);

            var (ok, message, hotelId) = await _hotelService.CreateAsync(dto, ImageFiles);
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
                Commission = hotel.Commission,
                Overview = hotel.Overview,
                Description = hotel.Description
            };

            return View(hotel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdateHotelDto dto, [FromForm] List<IFormFile> ImageFiles)
        {
            if (id != dto.HotelId) return NotFound();

            if (!ModelState.IsValid)
            {
                var hotelForView = await _context.TblHotels.AsNoTracking().FirstOrDefaultAsync(h => h.HotelId == id);
                return View(hotelForView);
            }

            if (ImageFiles != null && ImageFiles.Count > 0)
            {

                var file = ImageFiles.First();
                long maxFileSize = 5 * 1024 * 1024; 

                if (file.Length > maxFileSize)
                {
                    ModelState.AddModelError("ImageFiles", $"Error: File '{file.FileName}' is too large (Max: 5MB).");
                    var hotelForView = await _context.TblHotels.AsNoTracking().FirstOrDefaultAsync(h => h.HotelId == id);
                    return View(hotelForView);
                }

                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Hotels");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                string extension = Path.GetExtension(file.FileName);
                string uniqueFileName = $"{dto.HotelId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{extension}";
                string newFilePath = Path.Combine(uploadDir, uniqueFileName); 
                string newImageUrl = $"/images/Hotels/{uniqueFileName}"; 

                var firstOldImage = await _context.TblHotelImages.FirstOrDefaultAsync(img => img.HotelId == dto.HotelId);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                if (firstOldImage != null)
                {

                    if (!string.IsNullOrEmpty(firstOldImage.ImageUrl))
                    {

                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, firstOldImage.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    firstOldImage.ImageUrl = newImageUrl;
                    _context.TblHotelImages.Update(firstOldImage);
                }
                else
                {

                    var newImageRecord = new TblHotelImage
                    {
                        HotelId = dto.HotelId,
                        ImageUrl = newImageUrl
                    };
                    await _context.TblHotelImages.AddAsync(newImageRecord);
                }

                await _context.SaveChangesAsync();
            }

            var (ok, message) = await _hotelService.UpdateAsync(dto, null);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, message);
                var hotelForView = await _context.TblHotels.AsNoTracking().FirstOrDefaultAsync(h => h.HotelId == id);
                return View(hotelForView);
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