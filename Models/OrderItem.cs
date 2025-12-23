using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // O anki satış fiyatı
    }
}