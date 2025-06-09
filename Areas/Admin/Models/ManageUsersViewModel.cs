namespace _2280601466_NguyenNgocKhanh.Areas.Admin.Models
{
    public class ManageUsersViewModel
    {
        public List<UserRolesViewModel> Users { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();
    }

    public class UserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}