using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar görebilir
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Profil sayfası örneği (Giriş yapan kullanıcının adını gösterir)
        public IActionResult Profile()
        {
            var userName = User.Identity?.Name;
            return View("Profile", userName); // View'a string model gönderiyoruz, view tarafında düzelteceğiz
        }
    }
}