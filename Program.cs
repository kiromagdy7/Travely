using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Services.Hotels;
using Travely.Services.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hotels module services
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IImageStorage, FileSystemImageStorage>();

// Existing auth/cookie configuration assumed present

var app = builder.Build();

// Existing middleware pipeline assumed present
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();