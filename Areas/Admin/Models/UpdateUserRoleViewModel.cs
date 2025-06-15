using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace _2280601466_NguyenNgocKhanh.Areas.Admin.Models
{
    public class UpdateUserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CurrentRole { get; set; }
        public string NewRole { get; set; } = string.Empty;

        // ✅ Dùng SelectListItem thay vì string
        public List<SelectListItem> AllRoles { get; set; } = new();
    }
}
