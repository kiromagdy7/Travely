using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // Make sure this is included for [AllowAnonymous]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Make sure this is included for List<Claim>
using System.Security.Claims;
using System.Threading.Tasks;
using Travely.Data;
using Travely.Models;
using Travely.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Hosting; // <-- 1. ضيف ده
using System.IO; // <-- 2. ضيف ده

namespace Travely.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // <-- 3. ضيف السطر ده

        // 4. عدل الـ Constructor
        public AccountController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // <-- 5. ضيف السطر ده
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // لو اليوزر مسجل دخول أصلاً، وديه على الهوم
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // === START: Validation مهم جداً للأمان ===
            // نتأكد إن الدور المختار هو customer أو staff فقط
            if (model.Role != "customer" && model.Role != "staff")
            {
                ModelState.AddModelError("Role", "Invalid role selection.");
                return View(model);
            }
            // === END: Validation ===


            if (ModelState.IsValid)
            {
                if (await _context.TblUsers.AnyAsync(u => u.Email == model.Email))
                {
                    return RedirectToAction("Index", "Home");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var tblUser = new TblUser
                {
                    Fullname = model.Fullname,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    CreatedAt = DateTime.UtcNow,

                    // === التعديل هنا: ناخد الدور من الموديل ===
                    Role = model.Role, // <-- هياخد customer أو staff
                    Status = "active"
                };

                _context.Add(tblUser);
                await _context.SaveChangesAsync();

                // سجل دخوله علطول
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, tblUser.UserId.ToString()),
            new Claim(ClaimTypes.Name, tblUser.Fullname),
            new Claim(ClaimTypes.Email, tblUser.Email),
            new Claim(ClaimTypes.Role, tblUser.Role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous] // Allows access without being logged in
        public IActionResult Login()
        {
            // If user is already logged in, redirect them
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // Allows access without being logged in
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Find the user by email
                var user = await _context.TblUsers
                                 .FirstOrDefaultAsync(u => u.Email == model.Email);

                // 2. Check if user exists and is active
                if (user == null || user.Status != "active")
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt or account is inactive.");
                    return View(model);
                }

                // 3. Verify the password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

                if (isPasswordValid)
                {
                    // 4. Create Claims (User's identity info)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Fullname),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role) // <-- This is crucial for Authorization!
                    };

                    // 5. Create ClaimsIdentity
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // 6. Define Authentication Properties
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe, // "Remember Me"
                        // You can set other properties like ExpiresUtc here if needed
                    };

                    // 7. Sign in the user
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirect to home page
                    return RedirectToAction("Index", "Home");
                }

                // If password is invalid
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If model state is invalid
            return View(model);
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous] // Allow anyone to see the access denied page
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize] // لازم اليوزر يكون مسجل دخوله
        public async Task<IActionResult> Profile()
        {
            // 1. هات الـ ID بتاع اليوزر اللي مسجل دخوله حالياً
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(userIdString);

            // 2. هات بيانات اليوزر نفسه من الداتابيز
            var user = await _context.TblUsers.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // 3. هات حجوزات اليوزر (اللي فاتت فقط)
            var bookings = await _context.TblBookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                        .ThenInclude(h => h.TblHotelImages)
                .Where(b => b.UserId == userId && b.CheckOut < DateOnly.FromDateTime(DateTime.Now)) // استخدمنا الحل بتاع المرة اللي فاتت
                .OrderByDescending(b => b.CheckOut)
                .Select(b => new BookingInfoViewModel
                {
                    BookingId = b.BookingId,
                    HotelName = b.Room.Hotel.Name,
                    Location = b.Room.Hotel.Address,
                    // (TimeOnly.MinValue) يعني هتضيف وقت افتراضي (الساعة 12 بالليل)
                    StartDate = b.CheckIn.Value.ToDateTime(TimeOnly.MinValue),
                    EndDate = b.CheckOut.Value.ToDateTime(TimeOnly.MinValue),
                    Price = b.TotalPrice,
                    ImageUrl = b.Room.Hotel.TblHotelImages.FirstOrDefault().ImageUrl ?? "/images/default-hotel.png"
                })
                .ToListAsync();

            // 4. جهز الـ ViewModel الرئيسي (هنا التعديل)
            var viewModel = new ProfileViewModel
            {
                // === بداية التعديل: ملء البيانات الجديدة ===
                // افترضت إن الأسماء دي موجودة في (user) اللي هو TblUser
                Fullname = user.Fullname,
                Email = user.Email,
                Country = user.Country,
                Status = user.Status,
                Age = user.Age,
                Role = user.Role,
                Phone = user.Phone,
                Imagepath = user.Imagepath,
                // === نهاية التعديل ===

                PastBookings = bookings
            };

            // 5. ابعت الـ ViewModel للـ View
            return View(viewModel);
        }
        // GET: /Account/Edit
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // هات اليوزر الحالي
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.TblUsers.FindAsync(userId);

            if (user == null) return NotFound();

            // جهز الـ ViewModel بالبيانات الحالية
            var viewModel = new ProfileEditViewModel
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Country = user.Country,
                Age = user.Age,
                Phone = user.Phone,
                CurrentImagePath = user.Imagepath // ابعت مسار الصورة الحالية
            };

            return View(viewModel);
        }

        // POST: /Account/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // لو الفورم فيه أخطاء، ارجع تاني
            }

            // هات اليوزر من الداتابيز عشان نعدل عليه
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.TblUsers.FindAsync(userId);

            if (user == null) return NotFound();

            // --- 1. جزء معالجة الصورة ---
            if (model.NewImage != null && model.NewImage.Length > 0)
            {
                // 1.1: امسح الصورة القديمة (لو موجودة ومش هي الصورة الافتراضية)
                if (!string.IsNullOrEmpty(user.Imagepath) && user.Imagepath != "/images/default-avatar.png")
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.Imagepath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // 1.2: احفظ الصورة الجديدة
                // جهز المسار اللي هنحفظ فيه (مثلاً wwwroot/images/profiles)
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
                Directory.CreateDirectory(uploadsFolder); // اتأكد إن المجلد موجود

                // اعمل اسم فريد للصورة عشان ميحصلش تكرار
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(model.NewImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // احفظ الصورة في المسار ده
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewImage.CopyToAsync(fileStream);
                }

                // 1.3: حدث المسار في الداتابيز (لازم يبدأ بـ /)
                user.Imagepath = "/images/profiles/" + uniqueFileName;
            }

            // --- 2. تحديث باقي البيانات ---
            user.Fullname = model.Fullname;
            user.Email = model.Email;
            user.Country = model.Country;
            user.Age = (byte?)model.Age;
            user.Phone = model.Phone;

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!"; // رسالة نجاح
            return RedirectToAction("Edit"); // ارجع لنفس الصفحة عشان يشوف التعديلات
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Wishlist()
        {
            // 1. هات الـ ID بتاع اليوزر
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login");

            var userId = int.Parse(userIdString);

            // 2. هات بيانات اليوزر (عشان الـ Sidebar)
            var user = await _context.TblUsers.FindAsync(userId);
            if (user == null) return NotFound();

            // 3. هات الـ Wishlist بتاعته (عشان المحتوى)
            var wishlistItems = await _context.TblWishLists
                .Include(w => w.Hotels) // <-- افترضت إن اسمها Hotel
                    .ThenInclude(h => h.TblHotelImages) // <-- عشان نجيب الصورة
                .Where(w => w.UserId == userId) // <-- افترضت وجود UserId هنا
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            // 4. جهز الـ ViewModel
            var viewModel = new WishlistPageViewModel
            {
                // بيانات اليوزر للـ Sidebar
                Fullname = user.Fullname,
                Email = user.Email,
                Country = user.Country,
                Status = user.Status,
                Age = user.Age,
                Role = user.Role,
                Phone = user.Phone,
                Imagepath = user.Imagepath,

                // بيانات الـ Wishlist للمحتوى
                WishlistItems = wishlistItems
            };

            // 5. ابعت الـ ViewModel للـ View الجديد
            return View(viewModel); // هيدور على Views/Account/Wishlist.cshtml
        }
        // In your Startup.cs or Program.cs (depending on your ASP.NET Core version)
    } }