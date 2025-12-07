using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // Gerçek hayatta hashlenmeli ama okul projesi için düz metin olabilir (hocana danışabilirsin).
    }
}