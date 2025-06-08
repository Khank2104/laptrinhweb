using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace _2280601466_NguyenNgocKhanh.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public required string Name { get; set; } // Use 'required' to fix CS8618

        public List<Product>? Products { get; set; }
    }
}
