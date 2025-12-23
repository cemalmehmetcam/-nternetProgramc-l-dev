using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                // Identity ile kullanıcı adı (email) ve şifre kontrolü
                // UserName olarak Email kullandığımızı varsayıyorum.
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }

                ModelState.AddModelError("", "Hatalı e-posta veya şifre.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // Geçici olarak ilk Admin'i oluşturmak için bir metod (Bunu çalıştırdıktan sonra silebilirsin)
        [HttpGet]
        public async Task<IActionResult> CreateTestAdmin()
        {
            var user = new IdentityUser { UserName = "admin@test.com", Email = "admin@test.com", EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, "123"); // Şifre: 123

            if (result.Succeeded)
            {
                return Content("Admin oluşturuldu: admin@test.com / 123");
            }
            return Content("Hata: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}