using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using _2280601466_NguyenNgocKhanh.Models;

namespace _2280601466_NguyenNgocKhanh.Services
{
    public class CartDbService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartDbService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<List<CartItem>> GetCartAsync()
        {
            var userId = GetUserId();
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddToCartAsync(Product product, int quantity = 1)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == product.Id);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    UserId = userId,
                    Price = product.Price
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int productId)
        {
            var userId = GetUserId();
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync()
        {
            var userId = GetUserId();
            var items = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
