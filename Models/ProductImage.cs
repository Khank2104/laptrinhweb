using _2280601466_NguyenNgocKhanh.Models;

namespace _2280601466_NguyenNgocKhanh.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public required string Url { get; set; } // Use 'required' to fix CS8618
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
