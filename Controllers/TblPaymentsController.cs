using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;

namespace Travely.Controllers
{
    public class TblPaymentsController : Controller
    {
        private readonly AppDbContext _context;

        public TblPaymentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TblPayments
        public async Task<IActionResult> Index()
        {
            var payments = _context.TblPayments
                .Include(p => p.Booking)
                .Include(p => p.User);
            return View(await payments.ToListAsync());
        }

        // GET: TblPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tblPayment = await _context.TblPayments
                .Include(p => p.Booking)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);

            if (tblPayment == null) return NotFound();

            return View(tblPayment);
        }

        // GET: TblPayments/Create
        public IActionResult Create()
        {
            ViewData["BookingId"] = new SelectList(_context.TblBookings, "BookingId", "BookingReference");
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "Fullname");
            return View();
        }

        // POST: TblPayments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,BookingId,UserId,PaymentMethod,PaymentStatus,Amount,PaymentDate")] TblPayment tblPayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookingId"] = new SelectList(_context.TblBookings, "BookingId", "BookingReference", tblPayment.BookingId);
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "Fullname", tblPayment.UserId);
            return View(tblPayment);
        }

        // GET: TblPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tblPayment = await _context.TblPayments.FindAsync(id);
            if (tblPayment == null) return NotFound();

            ViewData["BookingId"] = new SelectList(_context.TblBookings, "BookingId", "BookingReference", tblPayment.BookingId);
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "Fullname", tblPayment.UserId);
            return View(tblPayment);
        }

        // POST: TblPayments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,BookingId,UserId,PaymentMethod,PaymentStatus,Amount,PaymentDate")] TblPayment tblPayment)
        {
            if (id != tblPayment.PaymentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblPaymentExists(tblPayment.PaymentId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookingId"] = new SelectList(_context.TblBookings, "BookingId", "BookingReference", tblPayment.BookingId);
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "Fullname", tblPayment.UserId);
            return View(tblPayment);
        }

        // GET: TblPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tblPayment = await _context.TblPayments
                .Include(p => p.Booking)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);

            if (tblPayment == null) return NotFound();

            return View(tblPayment);
        }

        // POST: TblPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblPayment = await _context.TblPayments.FindAsync(id);
            if (tblPayment != null)
            {
                _context.TblPayments.Remove(tblPayment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- New: Payment confirmation (GET)
        // GET: TblPayments/Confirm/5
        public async Task<IActionResult> Confirm(int? id)
        {
            if (id == null) return NotFound();

            var tblPayment = await _context.TblPayments
                .Include(p => p.Booking)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);

            if (tblPayment == null) return NotFound();

            return View(tblPayment);
        }

        // POST: TblPayments/Confirm/5
        // This approves/marks a payment as confirmed and redirects to a printable receipt.
        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmConfirmed(int id)
        {
            var tblPayment = await _context.TblPayments.FindAsync(id);
            if (tblPayment == null) return NotFound();

            // mark as confirmed
            tblPayment.PaymentStatus = "confirmed";
            // set the PaymentDate if it's default
            if (tblPayment.PaymentDate == default) tblPayment.PaymentDate = DateTime.UtcNow;

            _context.Update(tblPayment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Receipt), new { id = tblPayment.PaymentId });
        }

        // --- New: Receipt (GET)
        // GET: TblPayments/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            if (id == null) return NotFound();

            var tblPayment = await _context.TblPayments
                .Include(p => p.Booking)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PaymentId == id);

            if (tblPayment == null) return NotFound();

            return View(tblPayment);
        }

        private bool TblPaymentExists(int id)
        {
            return _context.TblPayments.Any(e => e.PaymentId == id);
        }
    }
}