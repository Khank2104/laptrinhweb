using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _2280601466_NguyenNgocKhanh.Models;
using System.Threading.Tasks;
using System.Linq;

namespace _2280601466_NguyenNgocKhanh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Where(o => o.Id == id)
                .FirstOrDefault();

            if (order == null)
                return NotFound();

            return View(order);
        }

        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
