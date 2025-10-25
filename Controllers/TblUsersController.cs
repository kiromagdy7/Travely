using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Travely.Data;
using Travely.Models;
using Travely.ViewModels;
using Microsoft.AspNetCore.Authorization; // <-- 1. Added this

namespace Travely.Controllers
{
    [Authorize(Roles = "admin")] // <-- 2. Added this to protect the whole controller
    public class TblUsersController : Controller
    {
        private readonly AppDbContext _context;

        public TblUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TblUsers
        public async Task<IActionResult> Index()
        {
            // جيب بس اليوزرز اللي حالتهم مش "محذوف"
            var users = await _context.TblUsers
                                      .Where(u => u.Status != "Deleted")
                                      .ToListAsync();
            return View(users);
        }

        // GET: TblUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblUser = await _context.TblUsers
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (tblUser == null || tblUser.Status == "Deleted")
            {
                return NotFound();
            }

            return View(tblUser);
        }

        // GET: TblUsers/Create
        public IActionResult Create()
        {
            // هنبعت ViewModel فاضي عشان نحدد القيم الافتراضية
            var viewModel = new TblUserCreateViewModel
            {
                Role = "customer",
                Status = "active"
            };
            return View(viewModel);
        }

        // POST: TblUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TblUserCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // 1. اتأكد إن الإيميل ده مش موجود قبل كده
                if (await _context.TblUsers.AnyAsync(u => u.Email == viewModel.Email))
                {
                    ModelState.AddModelError("Email", "This email is already in use.");
                    return View(viewModel);
                }

                // 2. اعمل Hashing للباسورد (تشفير)
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);

                // 3. حول الـ ViewModel لـ Model عادي
                var tblUser = new TblUser
                {
                    Fullname = viewModel.Fullname,
                    Email = viewModel.Email,
                    PasswordHash = hashedPassword, // <-- استخدم الباسورد المتشفر
                    Phone = viewModel.Phone,
                    Age = viewModel.Age,
                    Role = viewModel.Role,
                    Country = viewModel.Country,
                    Status = viewModel.Status,
                    Imagepath = viewModel.Imagepath,
                    CreatedAt = DateTime.UtcNow // <-- السيرفر هو اللي يحدد التاريخ
                };

                _context.Add(tblUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // لو الـ ModelState مش Valid، ارجع لنفس الـ View بالبيانات اللي اليوزر دخلها
            return View(viewModel);
        }

        // GET: TblUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblUser = await _context.TblUsers.FindAsync(id);
            if (tblUser == null || tblUser.Status == "Deleted")
            {
                return NotFound();
            }

            // 1. حول الـ Model لـ EditViewModel
            var viewModel = new TblUserEditViewModel
            {
                UserId = tblUser.UserId,
                Fullname = tblUser.Fullname,
                Email = tblUser.Email,
                Phone = tblUser.Phone,
                Age = tblUser.Age,
                Role = tblUser.Role,
                Country = tblUser.Country,
                Status = tblUser.Status,
                Imagepath = tblUser.Imagepath
                // مش هنبعت الباسورد للـ View
            };

            return View(viewModel);
        }

        // POST: TblUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TblUserEditViewModel viewModel)
        {
            if (id != viewModel.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // 1. اتأكد إن الإيميل مش متاخد (بواسطة يوزر تاني)
                if (await _context.TblUsers.AnyAsync(u => u.Email == viewModel.Email && u.UserId != viewModel.UserId))
                {
                    ModelState.AddModelError("Email", "This email is already in use by another user.");
                    return View(viewModel);
                }

                // 2. جيب اليوزر القديم من الداتابيز
                var userToUpdate = await _context.TblUsers.FindAsync(id);
                if (userToUpdate == null || userToUpdate.Status == "Deleted")
                {
                    return NotFound();
                }

                // 3. حدث البيانات بتاعته (Manual Mapping)
                userToUpdate.Fullname = viewModel.Fullname;
                userToUpdate.Email = viewModel.Email;
                userToUpdate.Phone = viewModel.Phone;
                userToUpdate.Age = viewModel.Age;
                userToUpdate.Role = viewModel.Role;
                userToUpdate.Country = viewModel.Country;
                userToUpdate.Status = viewModel.Status;
                userToUpdate.Imagepath = viewModel.Imagepath;

                // 4. حدث الباسورد (لو اليوزر كتب باسورد جديد بس)
                if (!string.IsNullOrEmpty(viewModel.Password))
                {
                    // شفر الباسورد الجديد
                    string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);
                    userToUpdate.PasswordHash = newHashedPassword;
                }

                try
                {
                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblUserExists(userToUpdate.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: TblUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblUser = await _context.TblUsers
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (tblUser == null || tblUser.Status == "Deleted")
            {
                return NotFound();
            }

            return View(tblUser);
        }

        // POST: TblUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblUser = await _context.TblUsers.FindAsync(id);
            if (tblUser != null)
            {
                // Soft Delete: غير الـ Status بدل ما تمسحه
                tblUser.Status = "Deleted";
                _context.Update(tblUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblUserExists(int id)
        {
            // هنشيل اليوزرز المحذوفين من البحث
            return _context.TblUsers.Any(e => e.UserId == id && e.Status != "Deleted");
        }
    }
}