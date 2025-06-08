using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2280601466_NguyenNgocKhanh.Models;

public class MockProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    public MockProductRepository()
    {
        // Tạo một số dữ liệu mẫu
        _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 1000, Description = "A high-end laptop" }
        };
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.AsEnumerable());
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task AddAsync(Product product)
    {
        // Nếu danh sách rỗng, gán Id = 1, ngược lại lấy Max + 1
        product.Id = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
        _products.Add(product);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            _products[index] = product;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _products.RemoveAll(p => p.Id == id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Product>> GetAllWithCategoryAndImagesAsync()
    {
        // Vì là mock nên chỉ trả về danh sách hiện tại, có thể gán thêm Category và Images nếu muốn
        foreach (var product in _products)
        {
            if (product.Category == null)
            {
                product.Category = new Category { Id = product.CategoryId, Name = $"Category {product.CategoryId}" };
            }
            if (product.Images == null)
            {
                product.Images = new List<ProductImage>();
            }
        }
        return Task.FromResult(_products.AsEnumerable());
    }

    public Task<Product?> GetByIdWithCategoryAndImagesAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            if (product.Category == null)
            {
                product.Category = new Category { Id = product.CategoryId, Name = $"Category {product.CategoryId}" };
            }
            if (product.Images == null)
            {
                product.Images = new List<ProductImage>();
            }
        }
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> SearchByNameAsync(string keyword)
    {
        var result = _products
            .Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        var result = _products
            .Where(p => p.CategoryId == categoryId)
            .AsEnumerable();
        return Task.FromResult(result);
    }
}