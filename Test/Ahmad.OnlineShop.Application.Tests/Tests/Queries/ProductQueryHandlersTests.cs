/// <summary>
/// تست‌های Application Query Handler محصول (ProductQueryHandlers)
/// پوشش‌دهنده: دریافت محصول، لیست محصولات، موجودی، تصاویر، دسته‌بندی‌ها
/// تکنولوژی Mock: NSubstitute — IProductRepository, ICategoryRepository
/// خطاهای تست‌شده: ProductNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Query.Handlers;
using Ahmad.OnlineShop.Application.Query.Queries;

namespace Ahmad.OnlineShop.Application.Query.Tests;

public class ProductQueryHandlersTests
{
    private readonly IProductRepository  _productRepo  = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepo = Substitute.For<ICategoryRepository>();
    private readonly ProductQueryHandlers _sut;
    private readonly CancellationToken    _ct = CancellationToken.None;

    public ProductQueryHandlersTests()
    {
        _sut = new ProductQueryHandlers(_productRepo, _categoryRepo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static Product MakeProduct() =>
        Product.Create(new CreateProductArg(1, 10, "Laptop", "Gaming", 50_000_000m, 1));

    // ─── GetProductQuery ──────────────────────────────────────────────────────

    /// <summary>دریافت محصول موجود باید اطلاعات صحیح برگرداند</summary>
    [Fact]
    public async Task GetProduct_When_Found_Should_ReturnProductResponse()
    {
        var product = MakeProduct();
        _productRepo.Get(1, _ct).Returns(product);

        var result = await _sut.HandleAsync(new GetProductQuery(1), _ct);

        Assert.Equal("Laptop",        result.Name);
        Assert.Equal(50_000_000m,     result.Price);
        Assert.Equal(10,              result.CategoryId);
        Assert.Equal(ProductStatus.Active, result.Status);
    }

    /// <summary>خطا: محصول پیدا نشد → ProductNotFoundException</summary>
    [Fact]
    public async Task GetProduct_When_NotFound_Should_Throw_ProductNotFoundException()
    {
        _productRepo.Get(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.HandleAsync(new GetProductQuery(99), _ct));
    }

    // ─── GetProductsQuery ─────────────────────────────────────────────────────

    /// <summary>لیست محصولات باید صفحه‌بندی درست داشته باشد</summary>
    [Fact]
    public async Task GetProducts_Should_ReturnPagedResult()
    {
        var products = new List<Product> { MakeProduct(), MakeProduct() };
        _productRepo.GetListAsync(1, 20, null, null, null, _ct).Returns((products, 2));

        var result = await _sut.HandleAsync(
            new GetProductsQuery(1, 20, null, null, null), _ct);

        Assert.Equal(2,     result.Items.Count);
        Assert.Equal(2,     result.TotalCount);
        Assert.Equal(1,     result.Page);
        Assert.Equal(20,    result.PageSize);
    }

    // ─── GetInventoryQuery ────────────────────────────────────────────────────

    /// <summary>دریافت موجودی باید داده صحیح از Inventory برگرداند</summary>
    [Fact]
    public async Task GetInventory_When_ProductFound_Should_ReturnInventoryResponse()
    {
        var product = MakeProduct();
        product.ReplenishStock(100);
        _productRepo.Get(1, _ct).Returns(product);

        var result = await _sut.HandleAsync(new GetInventoryQuery(1), _ct);

        Assert.Equal(100, result.Quantity);
        Assert.Equal(0,   result.ReservedQuantity);
        Assert.Equal(100, result.AvailableQuantity);
    }

    /// <summary>خطا: محصول پیدا نشد برای موجودی → ProductNotFoundException</summary>
    [Fact]
    public async Task GetInventory_When_ProductNotFound_Should_Throw_ProductNotFoundException()
    {
        _productRepo.Get(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.HandleAsync(new GetInventoryQuery(99), _ct));
    }

    // ─── GetProductImagesQuery ────────────────────────────────────────────────

    /// <summary>دریافت تصاویر باید لیست تصاویر را برگرداند</summary>
    [Fact]
    public async Task GetProductImages_When_ProductFound_Should_ReturnImages()
    {
        var product = MakeProduct();
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/1.jpg", "key1", ImageType.Gallery, 0));
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/2.jpg", "key2", ImageType.Gallery, 1));
        _productRepo.Get(1, _ct).Returns(product);

        var result = await _sut.HandleAsync(new GetProductImagesQuery(1), _ct);

        Assert.Equal(2, result.Count);
    }

    // ─── GetCategoriesQuery ───────────────────────────────────────────────────

    /// <summary>دریافت لیست دسته‌بندی‌ها باید همه آن‌ها را برگرداند</summary>
    [Fact]
    public async Task GetCategories_Should_ReturnAllCategories()
    {
        var categories = new List<Category>
        {
            new(1, "Electronics"),
            new(2, "Phones", 1)
        };
        _categoryRepo.Gets(_ct).Returns(categories);

        var result = await _sut.HandleAsync(new GetCategoriesQuery(), _ct);

        Assert.Equal(2,             result.Count);
        Assert.Equal("Electronics", result[0].Name);
        Assert.Null(result[0].ParentId);
        Assert.Equal(1,             result[1].ParentId);
    }
}
