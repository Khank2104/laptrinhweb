using _2280601466_NguyenNgocKhanh.Services;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly CartDbService _cartService;
    private readonly IProductRepository _productRepo;

    public CartController(CartDbService cartService, IProductRepository productRepo)
    {
        _cartService = cartService;
        _productRepo = productRepo;
    }

    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product != null)
        {
            await _cartService.AddToCartAsync(product, quantity);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        await _cartService.RemoveFromCartAsync(productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearCartAsync();
        return RedirectToAction("Index");
    }
}
