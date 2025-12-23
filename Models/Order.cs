namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } // Siparişi veren

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; } // Toplam Tutar

        public string OrderStatus { get; set; } = "Hazırlanıyor"; // Sipariş Durumu

        public List<OrderItem> OrderItems { get; set; } // Siparişin içindeki ürünler
    }
}