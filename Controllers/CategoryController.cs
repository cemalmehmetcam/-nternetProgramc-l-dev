using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Authorize] // Sadece giriş yapanlar görebilir
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // 1. LİSTELEME SAYFASI
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }

        // 2. EKLEME SAYFASI (Formu Göster)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2. EKLEME İŞLEMİ (Veriyi Kaydet)
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(category);
                _categoryRepository.Save();
                TempData["success"] = "Kategori başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // 3. DÜZENLEME SAYFASI (Mevcut Veriyi Getir)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0) return NotFound();

            var category = _categoryRepository.GetById(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // 3. DÜZENLEME İŞLEMİ (Güncellemeyi Kaydet)
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category);
                _categoryRepository.Save();
                TempData["success"] = "Kategori başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // 4. SİLME İŞLEMİ
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null) return NotFound();

            _categoryRepository.Delete(id);
            _categoryRepository.Save();
            TempData["success"] = "Kategori başarıyla silindi.";
            return RedirectToAction("Index");
        }
    }
}