using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Travely.Data;
using Travely.Models;
using Travely.ViewModels;

namespace Travely.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (model.Role != "customer" && model.Role != "staff")
            {
                ModelState.AddModelError("Role", "Invalid role selection.");
                return View(model);
            }

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
                    Phone = model.Phone,
                    PasswordHash = hashedPassword,
                    CreatedAt = DateTime.UtcNow,
                    Role = model.Role,
                    Status = "active"
                };

                _context.Add(tblUser);
                await _context.SaveChangesAsync();

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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.TblUsers.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null || user.Status != "active")
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt or account is inactive.");
                    return View(model);
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);


                if (isPasswordValid)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Fullname),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(userIdString);
            var user = await _context.TblUsers.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var bookings = await _context.TblBookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                        .ThenInclude(h => h.TblHotelImages)
                .Where(b => b.UserId == userId && b.CheckOut < DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(b => b.CheckOut)
                .Select(b => new BookingInfoViewModel
                {
                    BookingId = b.BookingId,
                    HotelName = b.Room.Hotel.Name,
                    Location = b.Room.Hotel.Address,
                    StartDate = b.CheckIn.Value.ToDateTime(TimeOnly.MinValue),
                    EndDate = b.CheckOut.Value.ToDateTime(TimeOnly.MinValue),
                    Price = b.TotalPrice,
                    ImageUrl = b.Room.Hotel.TblHotelImages.FirstOrDefault().ImageUrl ?? "/images/default-hotel.png"
                })
                .ToListAsync();

            var viewModel = new ProfileViewModel
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Country = user.Country,
                Status = user.Status,
                Age = user.Age,
                Role = user.Role,
                Phone = user.Phone,
                Imagepath = user.Imagepath,
                PastBookings = bookings
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.TblUsers.FindAsync(userId);

            if (user == null) return NotFound();

            var viewModel = new ProfileEditViewModel
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Country = user.Country,
                Age = user.Age,
                Phone = user.Phone,
                CurrentImagePath = user.Imagepath
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _context.TblUsers.FindAsync(userId);
                if (user == null) return NotFound();

                // ✅ أولاً: معالجة الصورة حتى لو البيانات ناقصة
                if (model.NewImage != null && model.NewImage.Length > 0)
                {
                    // تحقق من حجم الصورة
                    if (model.NewImage.Length > 5 * 1024 * 1024)
                    {
                        TempData["ErrorMessage"] = "حجم الصورة يجب ألا يتجاوز 5 ميجا بايت.";
                        return RedirectToAction("Edit");
                    }

                    // إنشاء مجلد الصور
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
                    Directory.CreateDirectory(uploadsFolder);

                    // استخراج الامتداد وتوليد اسم جديد
                    string extension = Path.GetExtension(model.NewImage.FileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string newFileName = $"{timestamp}{extension}";
                    string filePath = Path.Combine(uploadsFolder, newFileName);

                    // حفظ الصورة فعليًا
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewImage.CopyToAsync(fileStream);
                    }

                    // تحديث المسار
                    user.Imagepath = "/images/profiles/" + newFileName;
                }

                // ✅ ثانياً: حفظ باقي البيانات لو صالحة
                if (ModelState.IsValid)
                {
                    user.Fullname = model.Fullname;
                    user.Email = model.Email;
                    user.Country = model.Country;
                    user.Age = (byte?)model.Age;
                    user.Phone = model.Phone;

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "تم حفظ التعديلات بنجاح!";
                    return RedirectToAction("Edit");
                }

                // حتى لو البيانات ناقصة، نحفظ فقط الصورة اللي اتغيرت
                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم تحديث الصورة الشخصية بنجاح!";
                return RedirectToAction("Edit");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث الملف الشخصي. برجاء المحاولة لاحقًا.";
                return RedirectToAction("Edit");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Wishlist()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login");

            var userId = int.Parse(userIdString);
            var user = await _context.TblUsers.FindAsync(userId);
            if (user == null) return NotFound();

            var wishlistItems = await _context.TblWishLists
                .Include(w => w.Hotels)
                    .ThenInclude(h => h.TblHotelImages)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            var viewModel = new WishlistPageViewModel
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Country = user.Country,
                Status = user.Status,
                Age = user.Age,
                Role = user.Role,
                Phone = user.Phone,
                Imagepath = user.Imagepath,
                WishlistItems = wishlistItems
            };

            return View(viewModel);
        }
    }
}
