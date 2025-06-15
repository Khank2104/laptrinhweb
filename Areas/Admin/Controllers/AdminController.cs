using _2280601466_NguyenNgocKhanh.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                AllRoles = _roleManager.Roles.Select(r => r.Name ?? "").Where(name => !string.IsNullOrEmpty(name)).ToList()
            };

            return View(model);
        }

        // ✅ Hiển thị form cập nhật quyền
        [HttpGet]
        public async Task<IActionResult> EditUserRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(ManageUsers));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(ManageUsers));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var model = new UpdateUserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                CurrentRole = currentRoles.FirstOrDefault(),
                AllRoles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };

            return View(model);
        }


        // ✅ Xử lý cập nhật quyền (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRole(UpdateUserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(ManageUsers));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Cập nhật quyền thất bại.";
                return RedirectToAction(nameof(ManageUsers));
            }

            TempData["SuccessMessage"] = "Đã cập nhật quyền thành công.";
            return RedirectToAction(nameof(ManageUsers));
        }

        // ✅ Bước 1: Hiển thị trang xác nhận xóa
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(ManageUsers));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(ManageUsers));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                Roles = roles
            };

            return View(model);
        }

        // ✅ Bước 2: Xác nhận xóa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(ManageUsers));
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!removeRolesResult.Succeeded)
                {
                    TempData["ErrorMessage"] = "Không thể xóa roles: " + string.Join(", ", removeRolesResult.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(ManageUsers));
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Không thể xóa user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(ManageUsers));
            }

            TempData["SuccessMessage"] = "Đã xóa người dùng thành công.";
            return RedirectToAction(nameof(ManageUsers));
        }

        public IActionResult CreateUser()
        {
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                var roleResult = await _userManager.AddToRoleAsync(user, selectedRole);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", "Thêm role thất bại.");
                }

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

        public IActionResult ManageProducts()
        {
            return View();
        }
    }
}
