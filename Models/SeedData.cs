using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Veritabanı yoksa oluştur (Migrationları uygula)
                context.Database.Migrate();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // 1. "Admin" Rolü var mı? Yoksa oluştur.
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // 2. "Uye" Rolü var mı? Yoksa oluştur (Müşteriler için).
                if (!await roleManager.RoleExistsAsync("Uye"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Uye"));
                }

                // 3. Admin kullanıcısı var mı?
                var adminEmail = "admin@bakkal.com"; // Admin kullanıcı adı
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    // Admini oluştur (Şifre: 123)
                    var result = await userManager.CreateAsync(adminUser, "123");

                    if (result.Succeeded)
                    {
                        // Kullanıcıya "Admin" rolünü ver
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
}