using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Models;

namespace Travely.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> List(string role, int userId)
        {

            role = role?.ToLower() ?? "user";

            var notifications = await _context.TblNotifications
                .Where(n =>
                    n.NotificationType.ToLower() == "all" ||
                    (n.NotificationType.ToLower() == "admin" && role == "admin") ||
                    (n.NotificationType.ToLower() == "staff" && role == "staff") ||
                    (n.NotificationType.ToLower() == "user" && role == "user") ||
                    (n.UserId == userId)
                )
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return PartialView("NotificationList", notifications);
        }

        public async Task<IActionResult> Count(string role, int userId)
        {
            role = role?.ToLower() ?? "user";

            var count = await _context.TblNotifications
                .Where(n =>
                    !n.IsRead &&
                    (
                        n.NotificationType.ToLower() == "all" ||
                        (n.NotificationType.ToLower() == "admin" && role == "admin") ||
                        (n.NotificationType.ToLower() == "staff" && role == "staff") ||
                        (n.NotificationType.ToLower() == "user" && role == "user") ||
                        (n.UserId == userId)
                    )
                )
                .CountAsync();

            return Json(count);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notif = await _context.TblNotifications.FindAsync(id);
            if (notif != null)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        public async Task<IActionResult> Index()
        {
            var userRole = User.IsInRole("admin") ? "admin"
                          : User.IsInRole("staff") ? "staff"
                          : "user";

            var userIdClaim = User.FindFirst("UserId");
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            var notifications = await _context.TblNotifications
                .Where(n =>
                    n.NotificationType.ToLower() == "all" ||
                    (n.NotificationType.ToLower() == "admin" && userRole == "admin") ||
                    (n.NotificationType.ToLower() == "staff" && userRole == "staff") ||
                    (n.NotificationType.ToLower() == "user" && userRole == "user") ||
                    (n.UserId == userId)
                )
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }
        [Authorize(Roles = "admin,staff")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Message,NotificationType")] TblNotification notification)
        {
            if (!ModelState.IsValid)
                return View(notification);

            notification.UserId = null;               
            notification.IsRead = false;          
            notification.CreatedAt = DateTime.UtcNow;  

            try
            {
                _context.TblNotifications.Add(notification);
                await _context.SaveChangesAsync();

                TempData["ToastSuccess"] = "Notification sent successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty,
                    "Database error: " + (ex.InnerException?.Message ?? ex.Message));
                return View(notification);
            }
        }
    }
}
