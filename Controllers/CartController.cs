using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize] // Sadece giriş yapmış üyeler sepeti kullanabilir
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Sepeti Görüntüle
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product) // Ürün bilgilerini de getir
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            return View(cartItems);
        }

        // Sepete Ekle
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            // Ürün zaten sepette var mı?
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == productId);

            if (cartItem != null)
            {
                // Varsa adeti arttır
                cartItem.Quantity += quantity;
            }
            else
            {
                // Yoksa yeni ekle
                cartItem = new CartItem
                {
                    UserId = user.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); // Ekledikten sonra sepet sayfasına git
        }

        // Sepetten Sil
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Sepeti Temizle (Siparişten sonra lazım olacak)
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var cartItems = await _context.CartItems.Where(c => c.UserId == user.Id).ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}