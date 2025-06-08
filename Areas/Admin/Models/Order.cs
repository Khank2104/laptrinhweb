using System;
using System.ComponentModel.DataAnnotations;

namespace _2280601466_NguyenNgocKhanh.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        // Nếu bạn có danh sách sản phẩm trong đơn hàng:
        // public List<OrderItem> OrderItems { get; set; }
    }
}
