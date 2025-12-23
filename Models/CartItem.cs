using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Quantity { get; set; }

        // Sepet kime ait? (Giriş yapan kullanıcının ID'si)
        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}