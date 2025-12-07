using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Authorize] // Bu satır sayesinde sadece giriş yapmış (Cookie'si olan) kişiler buraya erişebilir
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Views/Admin/Index.cshtml dosyasını açar
        }
    }
}