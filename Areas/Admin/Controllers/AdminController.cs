using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _2280601466_NguyenNgocKhanh.Areas.Admin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2280601466_NguyenNgocKhanh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Hiển thị danh sách user và roles
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    Roles = roles
                });
            }

            var model = new ManageUsersViewModel
            {
                Users = userRolesViewModel,
                AllRoles = _roleManager.Roles
                    .Select(r => r.Name ?? "")
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList()
            };

            return View(model);
        }

        // Cập nhật role cho user
        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(string userId, string selectedRole)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(selectedRole))
                return BadRequest("UserId hoặc Role không được để trống");

            if (!await _roleManager.RoleExistsAsync(selectedRole))
                return BadRequest("Role không tồn tại");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User không tồn tại");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var result = await _userManager.AddToRoleAsync(user, selectedRole);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cập nhật role thất bại");
                return RedirectToAction(nameof(ManageUsers));
            }

            TempData["SuccessMessage"] = "Cập nhật quyền thành công.";
            return RedirectToAction(nameof(ManageUsers));
        }

        // Hiển thị form tạo tài khoản
        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        // Xử lý tạo tài khoản mới
        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string password, string selectedRole)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(selectedRole))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View();
            }

            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, selectedRole);
                TempData["SuccessMessage"] = "Tạo người dùng thành công.";
                return RedirectToAction(nameof(ManageUsers));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        // Xóa tài khoản
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Xóa người dùng thất bại.";
            }
            else
            {
                TempData["SuccessMessage"] = "Đã xóa người dùng.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        public IActionResult ManageProducts()
        {
            return View();
        }
    }
}
