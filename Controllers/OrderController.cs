using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR; // SignalR kütüphanesi eklendi
using Microsoft.EntityFrameworkCore;
using WebApplication1.Hubs; // Hub sınıfımızın olduğu yer
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize] // Sadece giriş yapmış üyeler sipariş verebilir
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<OrderHub> _hubContext; // SignalR HubContext tanımı

        // Constructor'a IHubContext eklendi
        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        // Kullanıcının Geçmiş Siparişleri
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Sipariş Tamamlama (Checkout) Ekranı
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);

            // Sepetteki ürünleri getir
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                TempData["Message"] = "Sepetiniz boş, sipariş verilemez.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(string address)
        {
            var user = await _userManager.GetUserAsync(User);

            // Sepeti getir (Product tablosunu dahil ederek)
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (cartItems.Count == 0) return RedirectToAction("Index", "Cart");

            // --- STOK KONTROLÜ VE DÜŞME İŞLEMİ ---
            foreach (var item in cartItems)
            {
                // 1. Stok Yeterli mi?
                if (item.Product.Stock < item.Quantity)
                {
                    TempData["Error"] = $"Hata: {item.Product.Name} ürününden stokta sadece {item.Product.Stock} adet kaldı.";
                    return RedirectToAction("Index", "Cart");
                }

                // 2. Stoktan Düş ve Satılanı Arttır
                item.Product.Stock -= item.Quantity;
                item.Product.SoldCount += item.Quantity;

                // Product tablosunu güncellemek için işaretle
                _context.Products.Update(item.Product);
            }
            // -------------------------------------

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                OrderStatus = "Hazırlanıyor",
                TotalAmount = cartItems.Sum(x => x.Quantity * x.Product.Price),
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync(); // Hem siparişi kaydeder hem de ürün stoklarını günceller

            // SignalR Bildirimi
            await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", $"Yeni Sipariş! Tutar: {order.TotalAmount:C}");

            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            return View();
        }
    }
}