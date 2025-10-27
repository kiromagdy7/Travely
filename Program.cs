using Microsoft.AspNetCore.Authentication.Cookies; // <-- 1. ضيف السطر ده
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Extensions;

namespace Travely
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
                ));
            // --- 2. ضيف خدمات الكوكيز هنا ---
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // مدة الكوكى
                    options.LoginPath = "/Account/Login"; // يروح فين لو مش مسجل دخول
                    options.AccessDeniedPath = "/Account/AccessDenied"; // يروح فين لو معندوش صلاحية
                    options.SlidingExpiration = true;
                });
            // ------------------------------------

            // Booking module registration (clean architecture: inject services)
            builder.Services.AddBookingModule();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // --- 3. تعديل ترتيب السطور دي ---
            app.MapStaticAssets(); // خليه هنا أو بدله بـ app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // <-- 4. ضيف السطر ده (قبل Authorization)
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets(); // (WithStaticAssets() دي غريبة شوية، بس هنسيبها زي ما هي)


            app.Run();
        }
    }
}