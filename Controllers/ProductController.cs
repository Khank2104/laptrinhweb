using _2280601466_NguyenNgocKhanh.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks; 
using _2280601466_NguyenNgocKhanh.Repositories; // Thay thế bằng namespace thực tế của bạn

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // Hiển thị danh sách sản phẩm
    public async Task<IActionResult> Index(int? categoryId)
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        IEnumerable<Product> products;

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            products = await _productRepository.GetByCategoryIdAsync(categoryId.Value);
        }
        else
        {
            products = await _productRepository.GetAllAsync();
        }

        return View(products);
    }


    // Hiển thị form thêm sản phẩm mới
    public async Task<IActionResult> Add()
    {
        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    // Xử lý thêm sản phẩm mới
    [HttpPost]
    public async Task<IActionResult> Add(Product product, IFormFile imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/images/" + fileName;
            }

            await _productRepository.AddAsync(product);
            TempData["Message"] = "Product added successfully!";
            return RedirectToAction(nameof(Add));
        }

        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(product);
    }

    // Hiển thị thông tin chi tiết sản phẩm
    public async Task<IActionResult> Display(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAndImagesAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // Hiển thị form cập nhật sản phẩm
    public async Task<IActionResult> Update(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
        return View(product);
    }

    // Xử lý cập nhật sản phẩm
    [HttpPost]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _productRepository.UpdateAsync(product);
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    // Hiển thị form xác nhận xóa sản phẩm
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // Xử lý xóa sản phẩm
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}