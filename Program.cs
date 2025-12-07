using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Repositories; // Repository namespace'ini eklemeyi unutma

var builder = WebApplication.CreateBuilder(args);

// 1. SQL Server Baðlantýsý (Yönerge gereði SQLite yerine SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repository Dependency Injection Tanýmlarý
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriþ yapmamýþ kiþi buraya atýlacak
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // 20 dk sonra oturum düþer
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ... (Geri kalan kýsýmlar ayný kalabilir) ...

app.UseHttpsRedirection();
app.UseStaticFiles(); // Bu eksikti, CSS/JS için gerekli

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();