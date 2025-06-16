using System.Collections.Generic;
using System.Threading.Tasks;
using _2280601466_NguyenNgocKhanh.Models;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<IEnumerable<Product>> GetAllWithCategoryAndImagesAsync();
    Task<Product?> GetByIdWithCategoryAndImagesAsync(int id);
    Task<IEnumerable<Product>> SearchByNameAsync(string keyword);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);

}
