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

namespace Travely.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
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
                    ModelState.AddModelError("Email", "This email is already in use.");
                    return View(model);
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
    }
}