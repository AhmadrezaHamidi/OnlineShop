/// <summary>
/// تست‌های Application Handler محصول (ProductHandlers)
/// پوشش‌دهنده: ایجاد/آپدیت محصول، وضعیت، دسته‌بندی، موجودی، تصویر
/// تکنولوژی Mock: NSubstitute — IProductRepository, ICategoryRepository
/// خطاهای تست‌شده: ProductNotFoundException | CategoryNotFoundException
///                  EmptyCategoryNameException
/// </summary>
using Ahmad.OnlineShop.Application.BackOffice;

namespace Ahmad.OnlineShop.Application.Handlers.Tests;

public class ProductHandlersTests
{
    private readonly IProductRepository  _productRepo  = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepo = Substitute.For<ICategoryRepository>();
    private readonly ProductHandlers     _sut;
    private readonly CancellationToken   _ct = CancellationToken.None;

    public ProductHandlersTests()
    {
        _sut = new ProductHandlers(_productRepo, _categoryRepo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static Category  MakeCategory() => new(10, "Electronics");
    private static Product   MakeProduct()  => Product.Create(
        new CreateProductArg(1, 10, "Laptop", null, 50_000_000m, 1));

    // ─── CreateProductCommand ─────────────────────────────────────────────────

    /// <summary>ایجاد محصول باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task Create_When_CategoryExists_Should_AddProduct()
    {
        _categoryRepo.Get(10, _ct).Returns(MakeCategory());
        _productRepo.GetNextId().Returns(1L);

        var result = await _sut.Handle(
            new CreateProductCommand(10, "Laptop", null, 50_000_000m), _ct);

        await _productRepo.Received(1).AddAsync(Arg.Any<Product>(), _ct);
    }

    /// <summary>خطا: دسته‌بندی پیدا نشد → CategoryNotFoundException</summary>
    [Fact]
    public async Task Create_When_CategoryNotFound_Should_Throw_CategoryNotFoundException()
    {
        _categoryRepo.Get(10, _ct).ReturnsNull();

        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => _sut.Handle(new CreateProductCommand(10, "Laptop", null, 50_000_000m), _ct));

        await _productRepo.DidNotReceive().AddAsync(Arg.Any<Product>(), _ct);
    }

    // ─── UpdateProductCommand ─────────────────────────────────────────────────

    /// <summary>آپدیت محصول باید اطلاعات را تغییر داده و Repository را بروزرسانی کند</summary>
    [Fact]
    public async Task Update_When_ProductAndCategoryExist_Should_UpdateAndSave()
    {
        var product = MakeProduct();
        _productRepo.Get(1, _ct).Returns(product);
        _categoryRepo.Get(20, _ct).Returns(new Category(20, "Phones"));

        await _sut.Handle(new UpdateProductCommand(1, "New Name", "New Desc", 20), _ct);

        Assert.Equal("New Name", product.Name);
        Assert.Equal("New Desc", product.Description);
        Assert.Equal(20,          product.CategoryId);
        await _productRepo.Received(1).UpdateAsync(product, _ct);
    }

    /// <summary>خطا: محصول پیدا نشد → ProductNotFoundException</summary>
    [Fact]
    public async Task Update_When_ProductNotFound_Should_Throw_ProductNotFoundException()
    {
        _productRepo.Get(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.Handle(new UpdateProductCommand(99, "Name", null, 10), _ct));
    }

    // ─── ActivateProductCommand ───────────────────────────────────────────────

    /// <summary>فعال‌سازی محصول غیرفعال باید وضعیت را Active کند</summary>
    [Fact]
    public async Task Activate_When_ProductInactive_Should_ActivateProduct()
    {
        var product = MakeProduct();
        product.Deactivate();
        _productRepo.Get(1, _ct).Returns(product);

        await _sut.Handle(new ActivateProductCommand(1), _ct);

        Assert.Equal(ProductStatus.Active, product.Status);
        await _productRepo.Received(1).UpdateAsync(product, _ct);
    }

    /// <summary>خطا: محصول پیدا نشد برای فعال‌سازی → ProductNotFoundException</summary>
    [Fact]
    public async Task Activate_When_ProductNotFound_Should_Throw_ProductNotFoundException()
    {
        _productRepo.Get(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.Handle(new ActivateProductCommand(99), _ct));
    }

    // ─── DeactivateProductCommand ─────────────────────────────────────────────

    /// <summary>غیرفعال‌سازی محصول باید وضعیت را Inactive کند</summary>
    [Fact]
    public async Task Deactivate_When_ProductActive_Should_DeactivateProduct()
    {
        var product = MakeProduct();
        _productRepo.Get(1, _ct).Returns(product);

        await _sut.Handle(new DeactivateProductCommand(1), _ct);

        Assert.Equal(ProductStatus.Inactive, product.Status);
    }

    // ─── ArchiveProductCommand ────────────────────────────────────────────────

    /// <summary>بایگانی محصول باید وضعیت را Archived کند</summary>
    [Fact]
    public async Task Archive_When_ProductActive_Should_ArchiveProduct()
    {
        var product = MakeProduct();
        _productRepo.Get(1, _ct).Returns(product);

        await _sut.Handle(new ArchiveProductCommand(1), _ct);

        Assert.Equal(ProductStatus.Archived, product.Status);
    }

    // ─── ReserveStockCommand ──────────────────────────────────────────────────

    /// <summary>رزرو موجودی باید مقدار رزرو را افزایش دهد</summary>
    [Fact]
    public async Task ReserveStock_When_ProductExists_Should_ReserveInventory()
    {
        var product = MakeProduct();
        product.ReplenishStock(100);
        _productRepo.Get(1, _ct).Returns(product);

        await _sut.Handle(new ReserveStockCommand(1, 30), _ct);

        Assert.Equal(30, product.Inventory.ReservedQuantity);
        await _productRepo.Received(1).UpdateAsync(product, _ct);
    }

    // ─── AddProductImageCommand ───────────────────────────────────────────────

    /// <summary>افزودن تصویر باید تصویر را به محصول اضافه کند</summary>
    [Fact]
    public async Task AddImage_When_ProductExists_Should_AddImageToProduct()
    {
        var product = MakeProduct();
        _productRepo.Get(1, _ct).Returns(product);

        var cmd = new AddProductImageCommand(1, "https://img.com/1.jpg", "bucket/key", ImageType.Gallery);
        await _sut.Handle(cmd, _ct);

        Assert.Single(product.Images);
        await _productRepo.Received(1).UpdateAsync(product, _ct);
    }

    // ─── CreateCategoryCommand ────────────────────────────────────────────────

    /// <summary>ایجاد دسته‌بندی جدید باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task CreateCategory_When_NameNotExists_Should_AddCategory()
    {
        _categoryRepo.ExistsByName("Electronics", _ct).Returns(false);
        _categoryRepo.GetNextId().Returns(1L);

        var result = await _sut.Handle(new CreateCategoryCommand("Electronics", null), _ct);

        await _categoryRepo.Received(1).Add(Arg.Any<Category>(), _ct);
    }

    /// <summary>خطا: نام دسته‌بندی تکراری → EmptyCategoryNameException</summary>
    [Fact]
    public async Task CreateCategory_When_NameExists_Should_Throw_EmptyCategoryNameException()
    {
        _categoryRepo.ExistsByName("Electronics", _ct).Returns(true);

        await Assert.ThrowsAsync<EmptyCategoryNameException>(
            () => _sut.Handle(new CreateCategoryCommand("Electronics", null), _ct));
    }
}
