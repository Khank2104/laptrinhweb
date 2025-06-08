using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace _2280601466_NguyenNgocKhanh.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public required string Name { get; set; } // Use 'required' to fix CS8618

        [Range(0.01, 1000000000000.00)]
        public decimal Price { get; set; }

        public required string Description { get; set; } // Use 'required' to fix CS8618

        public string? ImageUrl { get; set; }

        public List<ProductImage>? Images { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}