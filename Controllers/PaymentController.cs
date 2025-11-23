using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Travely.Data;
using Microsoft.EntityFrameworkCore;

namespace Travely.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateCheckoutSession(int bookingId)
        {
            // 1. Fetch booking to get price and room details
            var booking = await _context.TblBookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return NotFound();

            // 2. Define the domain (Ensure port 7215 is correct for your local run)
            var domain = "https://localhost:7215";

            // 3. Configure Stripe Session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            // Stripe expects amount in cents (e.g., $10.00 = 1000)
                            UnitAmount = (long)(booking.TotalPrice * 100),
                            Currency = "usd", // Change to "egp" if your Stripe account supports it
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = booking.Room.RoomType,
                                Description = $"Booking Reference: {booking.BookingId}"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                // Pass bookingId to Success to update DB later
                SuccessUrl = domain + $"/Payment/Success?bookingId={bookingId}",
                CancelUrl = domain + $"/Payment/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            // 4. Redirect user to Stripe
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> Success(int bookingId)
        {
            // 1. Retrieve the booking
            var booking = await _context.TblBookings.FindAsync(bookingId);

            if (booking == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // 2. Update the status in the Database
            // Ensure your TblBooking model has a 'Status' string property
            booking.Status = "Confirmed";

            // If you have an IsPaid boolean, uncomment this:
            // booking.IsPaid = true; 

            // 3. Save Changes
            _context.TblBookings.Update(booking);
            await _context.SaveChangesAsync();

            // 4. Show the Success View
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}