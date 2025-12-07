using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }

        // Bir kategoride birden çok ürün olabilir
        public virtual ICollection<Product> Products { get; set; }
    }
}