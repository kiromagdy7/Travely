using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;

namespace Travely.Controllers
{
    // === التعديل هنا: اسمح للأدمن والاستاف ===
    [Authorize(Roles = "admin, staff")]
    public class HotelsController : Controller
    {
        private readonly AppDbContext _context;

        public HotelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Hotels
        // === التعديل هنا: اسمح للكل ===
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblHotels.ToListAsync());
        }

        // GET: Hotels/Details/5
        // === التعديل هنا: اسمح للكل ===
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tblHotel = await _context.TblHotels.FirstOrDefaultAsync(m => m.HotelId == id);
            if (tblHotel == null) return NotFound();
            return View(tblHotel);
        }

        // GET: Hotels/Create (مقفولة للأدمن والاستاف فقط)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create (مقفولة للأدمن والاستاف فقط)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address,Location,Stars,Phone,ContactInfo,CheckInTime,CheckOutTime,CancellationPolicy,Fees,Commission")] TblHotel tblHotel)
        {
            if (ModelState.IsValid)
            {
                if (await _context.TblHotels.AnyAsync(h => h.Name == tblHotel.Name))
                {
                    ModelState.AddModelError("Name", "This hotel name is already in use.");
                    return View(tblHotel);
                }
                tblHotel.CreatedAt = DateTime.UtcNow;
                _context.Add(tblHotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblHotel);
        }

        // GET: Hotels/Edit/5 (مقفولة للأدمن والاستاف فقط)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tblHotel = await _context.TblHotels.FindAsync(id);
            if (tblHotel == null) return NotFound();
            return View(tblHotel);
        }

        // POST: Hotels/Edit/5 (مقفولة للأدمن والاستاف فقط)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelId,Name,Address,Location,Stars,Phone,ContactInfo,CheckInTime,CheckOutTime,CancellationPolicy,Fees,Commission,CreatedAt")] TblHotel tblHotel)
        {
            if (id != tblHotel.HotelId) return NotFound();
            if (ModelState.IsValid)
            {
                if (await _context.TblHotels.AnyAsync(h => h.Name == tblHotel.Name && h.HotelId != id))
                {
                    ModelState.AddModelError("Name", "This hotel name is already in use by another hotel.");
                    return View(tblHotel);
                }
                try
                {
                    _context.Update(tblHotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblHotelExists(tblHotel.HotelId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tblHotel);
        }

        // GET: Hotels/Delete/5 (مقفولة للأدمن والاستاف فقط)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tblHotel = await _context.TblHotels.FirstOrDefaultAsync(m => m.HotelId == id);
            if (tblHotel == null) return NotFound();
            return View(tblHotel);
        }

        // POST: Hotels/Delete/5 (مقفولة للأدمن والاستاف فقط)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblHotel = await _context.TblHotels.FindAsync(id);
            if (tblHotel != null)
            {
                _context.TblHotels.Remove(tblHotel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TblHotelExists(int id)
        {
            return _context.TblHotels.Any(e => e.HotelId == id);
        }
    }
}