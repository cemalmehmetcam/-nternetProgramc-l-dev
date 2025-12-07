using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;
using System.Diagnostics;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Admin> _adminRepository;
        public HomeController(ILogger<HomeController> logger, IRepository<Product> productRepository, IRepository<Admin> adminRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _adminRepository = adminRepository;
        }

        public IActionResult Index()
        {
            var adminList = _adminRepository.GetAll();
            if (!adminList.Any())
            {
                var newAdmin = new Admin
                {
                    Email = "admin@gmail.com",
                    Password = "1234"
                };
                _adminRepository.Add(newAdmin);
                _adminRepository.Save();
            }
            var products = _productRepository.GetAll();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}