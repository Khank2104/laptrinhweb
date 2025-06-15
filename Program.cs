using _2280601466_NguyenNgocKhanh.Models;
using _2280601466_NguyenNgocKhanh.Repositories;
using _2280601466_NguyenNgocKhanh.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------- SERVICES CONFIGURATION -----------------
builder.Services.AddControllersWithViews();

// Kết nối DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ Identity (có vai trò)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Razor Pages
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(); // Dùng để lưu giỏ hàng trong session
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Cần thiết cho CartService
builder.Services.AddScoped<CartDbService>();

// Inject Repository
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

var app = builder.Build();

// ----------------- MIDDLEWARE CONFIGURATION -----------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Kích hoạt route cho khu vực (Areas)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ----------------- SEED DỮ LIỆU BAN ĐẦU -----------------
async Task SeedDataAsync(IServiceProvider serviceProvider)
{
    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Tự động migrate nếu chưa có
    await dbContext.Database.MigrateAsync();

    // Seed danh mục sản phẩm nếu chưa có
    if (!dbContext.Categories.Any())
    {
        dbContext.Categories.AddRange(
            new Category { Name = "Laptop" },
            new Category { Name = "Desktop" },
            new Category { Name = "Điện Thoại" },
            new Category { Name = "PC" }
        );
        await dbContext.SaveChangesAsync();
    }

    // Tạo các vai trò nếu chưa có
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Tạo user admin nếu chưa có
    var adminEmail = "admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            // Nếu lỗi khi tạo admin, log ra màn hình console
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating admin user: {error.Description}");
            }
        }
    }
}

// Gọi seed dữ liệu khi app khởi chạy
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await SeedDataAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi seed dữ liệu: {ex.Message}");
    }
}

app.Run();
