using System.Collections.Generic;
using System.Threading.Tasks;
using _2280601466_NguyenNgocKhanh.Models;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
    Task<Category?> GetWithProductsAsync(int id);
}
