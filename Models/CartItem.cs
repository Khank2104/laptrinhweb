using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2280601466_NguyenNgocKhanh.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }


        // Navigation
        public Product Product { get; set; } = null!;
    }
}
