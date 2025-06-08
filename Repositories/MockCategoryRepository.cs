using _2280601466_NguyenNgocKhanh.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2280601466_NguyenNgocKhanh.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categoryList;

        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Laptop" },
                new Category { Id = 2, Name = "Desktop" },
                new Category { Id = 3, Name = "Smartphone" },
                new Category { Id = 4, Name = "PC" }
            };
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return Task.FromResult(_categoryList.AsEnumerable());
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            var category = _categoryList.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(category);
        }

        public Task AddAsync(Category category)
        {
            _categoryList.Add(category);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Category category)
        {
            var existingCategory = _categoryList.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            _categoryList.RemoveAll(c => c.Id == id);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            var exists = _categoryList.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }

        public Task<Category?> GetWithProductsAsync(int id)
        {
            // Vì mock nên Products luôn null hoặc rỗng, có thể gán tạm để test
            var category = _categoryList.FirstOrDefault(c => c.Id == id);
            if (category != null && category.Products == null)
            {
                category.Products = new List<Product>(); // hoặc gán dữ liệu giả lập nếu muốn
            }
            return Task.FromResult(category);
        }
    }
}