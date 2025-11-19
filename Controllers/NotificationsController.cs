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

        // Helper: determine current role + userId
        private (string role, int userId) GetCurrentUser()
        {
            var userRole = User.IsInRole("admin") ? "admin"
                          : User.IsInRole("staff") ? "staff"
                          : "user";

            var userIdClaim = User.FindFirst("UserId");
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            return (userRole, userId);
        }

        // Helper: check if current user is allowed to see this notification
        private bool UserCanAccessNotification(TblNotification n, string role, int userId)
        {
            var type = n.NotificationType?.ToLower() ?? "";
            return
                type == "all" ||
                (type == "admin" && role == "admin") ||
                (type == "staff" && role == "staff") ||
                (type == "user" && role == "user") ||
                (n.UserId == userId);
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
            var (role, userId) = GetCurrentUser();

            var notif = await _context.TblNotifications.FindAsync(id);
            if (notif == null)
                return NotFound(new { message = "Notification not found" });

            if (!UserCanAccessNotification(notif, role, userId))
                return Forbid();

            if (!notif.IsRead)
            {
                notif.IsRead = true;
                await _context.SaveChangesAsync();
            }

            // Return updated unread count via same filtering logic
            var unread = await _context.TblNotifications
                .Where(n => !n.IsRead && UserCanAccessNotification(n, role, userId))
                .CountAsync();

            return Json(new { success = true, unread });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (role, userId) = GetCurrentUser();

            var notif = await _context.TblNotifications.FindAsync(id);
            if (notif == null)
                return NotFound(new { message = "Notification not found" });

            // Authorization: admin/staff can delete any; user can delete only their personal (direct) notifications.
            bool canDelete = User.IsInRole("admin") || User.IsInRole("staff") ||
                             (notif.UserId.HasValue && notif.UserId.Value == userId);

            if (!canDelete)
                return Forbid();

            _context.TblNotifications.Remove(notif);
            await _context.SaveChangesAsync();

            var unread = await _context.TblNotifications
                .Where(n => !n.IsRead && UserCanAccessNotification(n, role, userId))
                .CountAsync();

            return Json(new { success = true, unread });
        }

        public async Task<IActionResult> Index()
        {
            var (role, userId) = GetCurrentUser();

            var notifications = await _context.TblNotifications
                .Where(n => UserCanAccessNotification(n, role, userId))
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