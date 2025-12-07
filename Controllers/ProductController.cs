using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment; // Resim kaydetmek için gerekli

        public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }

public IActionResult Index(int? categoryId)
{

    ViewBag.CategoryList = new SelectList(_categoryRepository.GetAll(), "Id", "Name", categoryId);

    IEnumerable<Product> productList;


    if (categoryId != null && categoryId != 0)
    {

        productList = _productRepository.GetAll(u => u.CategoryId == categoryId, includeProps: "Category");
    }
    else
    {

        productList = _productRepository.GetAll(includeProps: "Category");
    }

    return View(productList);
}


        [HttpGet]
        public IActionResult Create()
        {

            ViewBag.CategoryList = new SelectList(_categoryRepository.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile? file)
        {

            ModelState.Remove("Category");

            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");

                    if (!Directory.Exists(productPath)) Directory.CreateDirectory(productPath);


                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }


                    product.ImageUrl = @"/images/products/" + fileName;
                }


                _productRepository.Add(product);
                _productRepository.Save();
                TempData["success"] = "Ürün başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = new SelectList(_categoryRepository.GetAll(), "Id", "Name");
            return View(product);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();

            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();


            ViewBag.CategoryList = new SelectList(_categoryRepository.GetAll(), "Id", "Name");
            return View(product);
        }


        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? file)
        {
            ModelState.Remove("Category");
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\', '/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    product.ImageUrl = @"/images/products/" + fileName;
                }
                _productRepository.Update(product);
                _productRepository.Save();
                TempData["success"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = new SelectList(_categoryRepository.GetAll(), "Id", "Name");
            return View(product);
        }

        [HttpDelete]
        public IActionResult DeleteAjax(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\', '/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            _productRepository.Delete(id);
            _productRepository.Save();

            return Json(new { success = true, message = "Ürün başarıyla silindi." });
        }
    }
}