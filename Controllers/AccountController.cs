using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Repositories; // Repository kullanacağız

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository<Admin> _adminRepository;

        public AccountController(IRepository<Admin> adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Veritabanında bu mail ve şifreye sahip yönetici var mı?
                // Not: Gerçek projede şifreler hashlenmeli, şimdilik düz metin bakıyoruz.
                var admin = _adminRepository.GetAll().FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                if (admin != null)
                {
                    // Giriş Başarılı, Cookie oluşturalım
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.Email),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Yönetici paneline yönlendir (Henüz yapmadık ama adı Admin olacak)
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı e-posta veya şifre.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}