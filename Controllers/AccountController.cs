using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Travely.Data;
using Travely.Models;
using Travely.ViewModels;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // --- Register ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
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
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (model.Role != "customer" && model.Role != "staff")
            {
                ModelState.AddModelError("Role", "Invalid role selection.");
            }

            if (!string.IsNullOrEmpty(model.Email) && await _context.TblUsers.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
            }

            if (model.ProfileImage != null)
            {
                if (model.ProfileImage.Length > 5 * 1024 * 1024) // 5MB limit
                {
                    ModelState.AddModelError("ProfileImage", "Image size cannot exceed 5MB.");
                }

                string extension = Path.GetExtension(model.ProfileImage.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ProfileImage", "Only .jpg, .jpeg, .png, or .gif files are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                string imagePath = "/images/profiles/Unknown_person.jpg";

                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    try
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
                        Directory.CreateDirectory(uploadsFolder);

                        string extension = Path.GetExtension(model.ProfileImage.FileName).ToLowerInvariant();
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ProfileImage.CopyToAsync(fileStream);
                        }
                        imagePath = "/images/profiles/" + uniqueFileName;
                    }
                    catch
                    {
                        ModelState.AddModelError("ProfileImage", "An error occurred while uploading the image.");
                        return View(model);
                    }
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
                    Status = "active",
                    Imagepath = imagePath
                };

                _context.Add(tblUser);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the user.");
                    return View(model);
                }


              
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, tblUser.UserId.ToString()),
                    new Claim(ClaimTypes.Name, tblUser.Fullname),
                    new Claim(ClaimTypes.Email, tblUser.Email),
                    new Claim(ClaimTypes.Role, tblUser.Role),
                    new Claim("ImagePath", tblUser.Imagepath ?? "")
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

        // --- Login ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated ?? false)
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
                var user = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Email == model.Email);

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
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("ImagePath", user.Imagepath ?? "")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe, // Use RememberMe from ViewModel
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


        // --- Logout ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- Access Denied ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        // --- Profile (My Bookings) ---
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            var user = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            // --- 🌟 AMEENDEE: Changed to UtcNow for consistency 🌟 ---
            var bookings = await _context.TblBookings
                .AsNoTracking()
                .Include(b => b.Room.Hotel.TblHotelImages)
                .Where(b => b.UserId == userId && b.CheckOut < DateOnly.FromDateTime(DateTime.UtcNow)) // Filter past bookings
                .OrderByDescending(b => b.CheckOut)
                .Select(b => new BookingInfoViewModel
                {
                    BookingId = b.BookingId,
                    HotelName = b.Room.Hotel.Name ?? "N/A",
                    Location = b.Room.Hotel.Address ?? "N/A",
                    StartDate = b.CheckIn.HasValue ? b.CheckIn.Value.ToDateTime(TimeOnly.MinValue) : default,
                    EndDate = b.CheckOut.HasValue ? b.CheckOut.Value.ToDateTime(TimeOnly.MinValue) : default,
                    Price = b.TotalPrice,
                    ImageUrl = b.Room.Hotel.TblHotelImages.Select(img => img.ImageUrl).FirstOrDefault() ?? "/images/default-hotel.png"
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
                PastBookings = bookings ?? new List<BookingInfoViewModel>()
            };

            return View(viewModel);
        }


        // --- Edit Profile (GET) ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            var user = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            ViewBag.Countries = GetCountryList();

            var viewModel = new ProfileEditViewModel
            {
                Fullname = user.Fullname ?? "",
                Email = user.Email ?? "",
                Country = user.Country,
                Age = user.Age,
                Phone = user.Phone,
                CurrentImagePath = user.Imagepath
            };

            return View(viewModel);
        }

        // --- Edit Profile (POST) - With Correct Claim Update ---
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            var user = await _context.TblUsers.FindAsync(userId);
            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            bool imageUpdated = false;
            string newImagePath = user.Imagepath;
            string extension = string.Empty; // Store extension here

            // --- 1. Process Image Upload ---
            if (model.NewImage != null && model.NewImage.Length > 0)
            {
                if (model.NewImage.Length > 5 * 1024 * 1024) // 5MB limit
                {
                    ModelState.AddModelError("NewImage", "Image size cannot exceed 5MB.");
                }

                // --- 🌟 AMEENDEE: Added Image Extension Validation 🌟 ---
                extension = Path.GetExtension(model.NewImage.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("NewImage", "Only .jpg, .jpeg, .png, or .gif files are allowed.");
                }
                // --- End Validation ---

                // Only proceed if there are no image-related errors so far
                if (!ModelState.ContainsKey("NewImage"))
                {
                    try
                    {
                        string oldImagePath = user.Imagepath; // Store old path before overwriting

                        // Save the new image
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + extension; // Use the validated extension
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.NewImage.CopyToAsync(fileStream);
                        }
                        newImagePath = "/images/profiles/" + uniqueFileName; // Update the path variable
                        imageUpdated = true;

                        // --- 🌟 AMEENDEE: Corrected Default Image Check 🌟 ---
                        string defaultImagePath = "/images/profiles/Unknown_person.jpg";
                        if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != defaultImagePath)
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                try { System.IO.File.Delete(oldFilePath); } catch { /* Log or ignore delete errors */ }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("NewImage", "Error uploading image.");
                        // Log ex.Message for details
                    }
                }
            }
            // Update the user entity's image path - this will be saved later if ModelState is valid
            user.Imagepath = newImagePath;


            // --- 2. Validate and Update Other User Data ---
            if (user.Email != model.Email && !string.IsNullOrEmpty(model.Email) &&
                await _context.TblUsers.AnyAsync(u => u.Email == model.Email && u.UserId != userId))
            {
                ModelState.AddModelError("Email", "This email address is already in use by another account.");
            }

            if (ModelState.IsValid)
            {
                user.Fullname = model.Fullname;
                user.Email = model.Email;
                user.Country = model.Country;
                user.Age = (byte?)model.Age;
                user.Phone = model.Phone;

                try
                {
                    await _context.SaveChangesAsync(); // Save all changes

                    // --- 3. Update Claims **AFTER** successful save ---
                    var currentPrincipal = (ClaimsPrincipal)User;
                    var currentAuthResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    var existingProperties = currentAuthResult?.Properties ?? new AuthenticationProperties();

                    var claims = currentPrincipal.Claims.Where(c =>
                        c.Type != ClaimTypes.Name &&
                        c.Type != ClaimTypes.Email &&
                        c.Type != "ImagePath"
                    ).ToList();

                    claims.Add(new Claim(ClaimTypes.Name, user.Fullname ?? ""));
                    claims.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));
                    claims.Add(new Claim("ImagePath", user.Imagepath ?? ""));

                    var newIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var newPrincipal = new ClaimsPrincipal(newIdentity);

                    var newAuthProperties = new AuthenticationProperties
                    {
                        IsPersistent = existingProperties.IsPersistent,
                        IssuedUtc = existingProperties.IssuedUtc,
                        ExpiresUtc = existingProperties.ExpiresUtc,
                        RedirectUri = existingProperties.RedirectUri,
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal, newAuthProperties);

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Edit");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError(string.Empty, "Concurrency error. The record was modified by another user. Please reload and try again.");
                    TempData["ErrorMessage"] = "Concurrency error. Please reload and try again.";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving profile changes.");
                    TempData["ErrorMessage"] = "Error saving profile. Please try again.";
                    // Log ex for details
                }
            }

            ViewBag.Countries = GetCountryList();
            model.CurrentImagePath = user.Imagepath;
            return View(model);
        }


        // --- Wishlist ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Wishlist()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            var user = await _context.TblUsers.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

            var wishlistItems = await _context.TblWishLists
                .AsNoTracking()
                .Include(w => w.Hotel)
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
                WishlistItems = wishlistItems ?? new List<TblWishList>()
            };

            return View(viewModel);
        }

        // --- Helper Method for Country List ---
        // --- 🌟 AMEENDEE: Fixed syntax errors in the provided list 🌟 ---
        private List<SelectListItem> GetCountryList()
        {
            // Comprehensive list of countries
            var countries = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Country --" },
                new SelectListItem { Value = "Afghanistan", Text = "Afghanistan" },
                new SelectListItem { Value = "Albania", Text = "Albania" },
                new SelectListItem { Value = "Algeria", Text = "Algeria" },
                new SelectListItem { Value = "Andorra", Text = "Andorra" },
                new SelectListItem { Value = "Angola", Text = "Angola" },
                new SelectListItem { Value = "Antigua and Barbuda", Text = "Antigua and Barbuda" },
                new SelectListItem { Value = "Argentina", Text = "Argentina" },
                new SelectListItem { Value = "Armenia", Text = "Armenia" },
                new SelectListItem { Value = "Australia", Text = "Australia" },
                new SelectListItem { Value = "Austria", Text = "Austria" },
                new SelectListItem { Value = "Azerbaijan", Text = "Azerbaijan" },
                new SelectListItem { Value = "Bahamas", Text = "Bahamas" },
                new SelectListItem { Value = "Bahrain", Text = "Bahrain" },
                new SelectListItem { Value = "Bangladesh", Text = "Bangladesh" },
                new SelectListItem { Value = "Barbados", Text = "Barbados" },
                new SelectListItem { Value = "Belarus", Text = "Belarus" },
                new SelectListItem { Value = "Belgium", Text = "Belgium" },
                new SelectListItem { Value = "Belize", Text = "Belize" },
                new SelectListItem { Value = "Benin", Text = "Benin" },
                new SelectListItem { Value = "Bhutan", Text = "Bhutan" },
                new SelectListItem { Value = "Bolivia", Text = "Bolivia" },
                new SelectListItem { Value = "Bosnia and Herzegovina", Text = "Bosnia and Herzegovina" },
                new SelectListItem { Value = "Botswana", Text = "Botswana" },
                new SelectListItem { Value = "Brazil", Text = "Brazil" },
                new SelectListItem { Value = "Brunei", Text = "Brunei" },
                new SelectListItem { Value = "Bulgaria", Text = "Bulgaria" },
                new SelectListItem { Value = "Burkina Faso", Text = "Burkina Faso" },
                new SelectListItem { Value = "Burundi", Text = "Burundi" },
                new SelectListItem { Value = "Cabo Verde", Text = "Cabo Verde" },
                new SelectListItem { Value = "Cambodia", Text = "Cambodia" },
                new SelectListItem { Value = "Cameroon", Text = "Cameroon" },
                new SelectListItem { Value = "Canada", Text = "Canada" },
                new SelectListItem { Value = "Central African Republic", Text = "Central African Republic" },
                new SelectListItem { Value = "Chad", Text = "Chad" },
                new SelectListItem { Value = "Chile", Text = "Chile" },
                new SelectListItem { Value = "China", Text = "China" },
                new SelectListItem { Value = "Colombia", Text = "Colombia" },
                new SelectListItem { Value = "Comoros", Text = "Comoros" },
                new SelectListItem { Value = "Congo, Democratic Republic of the", Text = "Congo, Democratic Republic of the" },
                new SelectListItem { Value = "Congo, Republic of the", Text = "Congo, Republic of the" },
                new SelectListItem { Value = "Costa Rica", Text = "Costa Rica" },
                new SelectListItem { Value = "Cote d'Ivoire", Text = "Cote d'Ivoire" },
                new SelectListItem { Value = "Dominican Republic", Text = "Dominican Republic" },
                new SelectListItem { Value = "Ecuador", Text = "Ecuador" },
                new SelectListItem { Value = "Egypt", Text = "Egypt" },
                new SelectListItem { Value = "El Salvador", Text = "El Salvador" },
                new SelectListItem { Value = "Equatorial Guinea", Text = "Equatorial Guinea" },
                new SelectListItem { Value = "Eritrea", Text = "Eritrea" },
                new SelectListItem { Value = "Estonia", Text = "Estonia" },
                new SelectListItem { Value = "Eswatini", Text = "Eswatini" },
                new SelectListItem { Value = "Ethiopia", Text = "Ethiopia" },
                new SelectListItem { Value = "Fiji", Text = "Fiji" },
                new SelectListItem { Value = "Finland", Text = "Finland" },
                new SelectListItem { Value = "France", Text = "France" },
                new SelectListItem { Value = "Gabon", Text = "Gabon" },
                new SelectListItem { Value = "Gambia", Text = "Gambia" },
                new SelectListItem { Value = "Georgia", Text = "Georgia" },
                new SelectListItem { Value = "Guinea-Bissau", Text = "Guinea-Bissau" },
                new SelectListItem { Value = "Guyana", Text = "Guyana" },
                new SelectListItem { Value = "Haiti", Text = "Haiti" },
                new SelectListItem { Value = "Honduras", Text = "Honduras" },
                new SelectListItem { Value = "Hungary", Text = "Hungary" },
                new SelectListItem { Value = "Iceland", Text = "Iceland" },
                new SelectListItem { Value = "India", Text = "India" },
                new SelectListItem { Value = "Indonesia", Text = "Indonesia" },
                new SelectListItem { Value = "Iran", Text = "Iran" },
                new SelectListItem { Value = "Kenya", Text = "Kenya" },
                new SelectListItem { Value = "Kiribati", Text = "Kiribati" },
                new SelectListItem { Value = "Kyrgyzstan", Text = "Kyrgyzstan" },
                new SelectListItem { Value = "Laos", Text = "Laos" },
                new SelectListItem { Value = "Latvia", Text = "Latvia" },
                new SelectListItem { Value = "Lebanon", Text = "Lebanon" },
                new SelectListItem { Value = "Maldives", Text = "Maldives" },
                new SelectListItem { Value = "Mali", Text = "Mali" },
                new SelectListItem { Value = "Malta", Text = "Malta" },
                new SelectListItem { Value = "Marshall Islands", Text = "Marshall Islands" },
                new SelectListItem { Value = "Mauritania", Text = "Mauritania" },
                new SelectListItem { Value = "Mauritius", Text = "Mauritius" },
                new SelectListItem { Value = "Mexico", Text = "Mexico" },
                new SelectListItem { Value = "North Macedonia", Text = "North Macedonia" },
                new SelectListItem { Value = "Norway", Text = "Norway" },
                new SelectListItem { Value = "Oman", Text = "Oman" },
                new SelectListItem { Value = "Pakistan", Text = "Pakistan" },
                new SelectListItem { Value = "Palau", Text = "Palau" },
                new SelectListItem { Value = "Palestine State", Text = "Palestine State" },
                new SelectListItem { Value = "Panama", Text = "Panama" },
                new SelectListItem { Value = "Qatar", Text = "Qatar" },
                new SelectListItem { Value = "Romania", Text = "Romania" },
                new SelectListItem { Value = "Russia", Text = "Russia" },
                new SelectListItem { Value = "Rwanda", Text = "Rwanda" },
                new SelectListItem { Value = "Seychelles", Text = "Seychelles" },
                new SelectListItem { Value = "Sierra Leone", Text = "Sierra Leone" },
                new SelectListItem { Value = "Taiwan", Text = "Taiwan" },
                new SelectListItem { Value = "Tajikistan", Text = "Tajikistan" },
                new SelectListItem { Value = "Tanzania", Text = "Tanzania" },
                new SelectListItem { Value = "United States", Text = "United States" },
                new SelectListItem { Value = "Uruguay", Text = "Uruguay" },
                new SelectListItem { Value = "Uzbekistan", Text = "Uzbekistan" },
                new SelectListItem { Value = "Vanuatu", Text = "Vanuatu" },
                new SelectListItem { Value = "Vatican City", Text = "Vatican City" },
                new SelectListItem { Value = "Venezuela", Text = "Venezuela" },
                new SelectListItem { Value = "Vietnam", Text = "Vietnam" },
                new SelectListItem { Value = "Yemen", Text = "Yemen" },
                new SelectListItem { Value = "Zambia", Text = "Zambia" },
                new SelectListItem { Value = "Zimbabwe", Text = "Zimbabwe" }
            };
            return countries.OrderBy(c => c.Value == "" ? 0 : 1).ThenBy(c => c.Text).ToList();
        }
    }
}