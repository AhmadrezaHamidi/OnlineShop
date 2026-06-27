/// <summary>
/// تست‌های Application Handler محصول (ProductHandlers)
/// پوشش‌دهنده: ایجاد/آپدیت محصول، وضعیت، دسته‌بندی، موجودی، تصویر
/// تکنولوژی: Fake Repository (بدون نیاز به mocking library)
/// خطاهای تست‌شده: ProductNotFoundException | CategoryNotFoundException | EmptyCategoryNameException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Handlers;

namespace Ahmad.OnlineShop.Application.Tests.Commands;

public class ProductHandlersTests
{
    private readonly FakeProductRepository  _productRepo  = new();
    private readonly FakeCategoryRepository _categoryRepo = new();
    private readonly ProductHandlers        _sut;
    private readonly CancellationToken      _ct = CancellationToken.None;

    public ProductHandlersTests()
    {
        _sut = new ProductHandlers(_productRepo, _categoryRepo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static CategoryAgg MakeCategory() => new(10, "Electronics");
    private static ProductAgg  MakeProduct()  => ProductAgg.Create(
        new CreateProductArg(1, 10, "Laptop", null, 50_000_000m, 1));

    // ─── CreateProductCommand ─────────────────────────────────────────────────

    /// <summary>ایجاد محصول باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task Create_When_CategoryExists_Should_AddProduct()
    {
        _categoryRepo.Seed(MakeCategory());

        await _sut.Handle(new CreateProductCommand(10, "Laptop", null, 50_000_000m), _ct);

        Assert.NotNull(_productRepo.Added);
        Assert.Equal("Laptop", _productRepo.Added!.Name);
    }

    /// <summary>خطا: دسته‌بندی پیدا نشد → CategoryNotFoundException</summary>
    [Fact]
    public async Task Create_When_CategoryNotFound_Should_Throw_CategoryNotFoundException()
    {
        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => _sut.Handle(new CreateProductCommand(10, "Laptop", null, 50_000_000m), _ct));

        Assert.Null(_productRepo.Added);
    }

    // ─── UpdateProductCommand ─────────────────────────────────────────────────

    /// <summary>آپدیت محصول باید اطلاعات را تغییر داده و Repository را بروزرسانی کند</summary>
    [Fact]
    public async Task Update_When_ProductAndCategoryExist_Should_UpdateAndSave()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);
        _categoryRepo.Seed(new CategoryAgg(20, "Phones"));

        await _sut.Handle(new UpdateProductCommand(product.Id, "New Name", "New Desc", 20), _ct);

        Assert.Equal("New Name", product.Name);
        Assert.Equal("New Desc", product.Description);
        Assert.Equal(20,          product.CategoryId);
    }

    /// <summary>خطا: محصول پیدا نشد → ProductNotFoundException</summary>
    [Fact]
    public async Task Update_When_ProductNotFound_Should_Throw_ProductNotFoundException()
    {
        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.Handle(new UpdateProductCommand(9999, "Name", null, 10), _ct));
    }

    // ─── ActivateProductCommand ───────────────────────────────────────────────

    /// <summary>فعال‌سازی محصول غیرفعال باید وضعیت را Active کند</summary>
    [Fact]
    public async Task Activate_When_ProductInactive_Should_ActivateProduct()
    {
        var product = MakeProduct();
        product.Deactivate();
        _productRepo.Seed(product);

        await _sut.Handle(new ActivateProductCommand(product.Id), _ct);

        Assert.Equal(ProductStatus.Active, product.Status);
    }

    /// <summary>خطا: محصول پیدا نشد → ProductNotFoundException</summary>
    [Fact]
    public async Task Activate_When_ProductNotFound_Should_Throw_ProductNotFoundException()
    {
        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.Handle(new ActivateProductCommand(9999), _ct));
    }

    // ─── DeactivateProductCommand ─────────────────────────────────────────────

    /// <summary>غیرفعال‌سازی محصول Active باید وضعیت را Inactive کند</summary>
    [Fact]
    public async Task Deactivate_When_ProductActive_Should_DeactivateProduct()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);

        await _sut.Handle(new DeactivateProductCommand(product.Id), _ct);

        Assert.Equal(ProductStatus.Inactive, product.Status);
    }

    // ─── ArchiveProductCommand ────────────────────────────────────────────────

    /// <summary>بایگانی محصول Active باید وضعیت را Archived کند</summary>
    [Fact]
    public async Task Archive_When_ProductActive_Should_ArchiveProduct()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);

        await _sut.Handle(new ArchiveProductCommand(product.Id), _ct);

        Assert.Equal(ProductStatus.Archived, product.Status);
    }

    // ─── ReserveStockCommand ──────────────────────────────────────────────────

    /// <summary>رزرو موجودی باید مقدار رزرو را افزایش دهد</summary>
    [Fact]
    public async Task ReserveStock_When_ProductExists_Should_ReserveInventory()
    {
        var product = MakeProduct();
        product.ReplenishStock(100);
        _productRepo.Seed(product);

        await _sut.Handle(new ReserveStockCommand(product.Id, 30), _ct);

        Assert.Equal(30, product.Inventory.ReservedQuantity);
    }

    // ─── AddProductImageCommand ───────────────────────────────────────────────

    /// <summary>افزودن تصویر باید تصویر را به محصول اضافه کند</summary>
    [Fact]
    public async Task AddImage_When_ProductExists_Should_AddImageToProduct()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);

        await _sut.Handle(
            new AddProductImageCommand(product.Id, "https://img.com/1.jpg", "bucket/key", ImageType.Gallery), _ct);

        Assert.Single(product.Images);
    }

    // ─── CreateCategoryCommand ────────────────────────────────────────────────

    /// <summary>ایجاد دسته‌بندی جدید باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task CreateCategory_When_NameNotExists_Should_AddCategory()
    {
        _categoryRepo.NameExists = false;

        await _sut.Handle(new CreateCategoryCommand("Electronics", null), _ct);

        Assert.NotNull(_categoryRepo.Added);
        Assert.Equal("Electronics", _categoryRepo.Added!.Name);
    }

    /// <summary>خطا: نام دسته‌بندی تکراری → EmptyCategoryNameException</summary>
    [Fact]
    public async Task CreateCategory_When_NameExists_Should_Throw_EmptyCategoryNameException()
    {
        _categoryRepo.NameExists = true;

        await Assert.ThrowsAsync<EmptyCategoryNameException>(
            () => _sut.Handle(new CreateCategoryCommand("Electronics", null), _ct));
    }
}
