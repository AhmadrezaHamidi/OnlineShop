/// <summary>
/// ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Application Handler Ù…Ø­ØµÙˆÙ„ (ProductHandlers)
/// Ù¾ÙˆØ´Ø´â€ŒØ¯Ù‡Ù†Ø¯Ù‡: Ø§ÛŒØ¬Ø§Ø¯/Ø¢Ù¾Ø¯ÛŒØª Ù…Ø­ØµÙˆÙ„ØŒ ÙˆØ¶Ø¹ÛŒØªØŒ Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒØŒ Ù…ÙˆØ¬ÙˆØ¯ÛŒØŒ ØªØµÙˆÛŒØ±
/// ØªÚ©Ù†ÙˆÙ„ÙˆÚ˜ÛŒ: Fake Repository (Ø¨Ø¯ÙˆÙ† Ù†ÛŒØ§Ø² Ø¨Ù‡ mocking library)
/// Ø®Ø·Ø§Ù‡Ø§ÛŒ ØªØ³Øªâ€ŒØ´Ø¯Ù‡: ProductNotFoundException | CategoryNotFoundException | EmptyCategoryNameException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Handlers;

namespace Ahmad.OnlineShop.Application.Tests.Commands;

public class ProductHandlersTests
{
    private readonly FakeProductRepository _productRepo = new();
    private readonly FakeCategoryRepository _categoryRepo = new();
    private readonly ProductHandlers _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ProductHandlersTests()
    {
        _sut = new ProductHandlers(_productRepo, _categoryRepo, FakeAppDb.Create());
    }

    // â”€â”€â”€ Helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static CategoryAgg MakeCategory() => new(10, "Electronics");
    private static ProductAgg MakeProduct() => ProductAgg.Create(
        new CreateProductArg(Id: 1, SellerId: 100, CategoryId: 10, Name: "Laptop", Description: null, Price: 50_000_000m, InventoryId: 1));

    private static CreateProductCommand MakeCreateCommand(long sellerId = 100) =>
        new(SellerId: sellerId, CategoryId: 10, Name: "Laptop", Description: null, Price: 50_000_000m);

    // â”€â”€â”€ CreateProductCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø§ÛŒØ¬Ø§Ø¯ Ù…Ø­ØµÙˆÙ„ Ø¨Ø§ÛŒØ¯ Ø¢Ù† Ø±Ø§ Ø¯Ø± Repository Ø°Ø®ÛŒØ±Ù‡ Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Create_When_CategoryExists_Should_AddProduct()
    {
        _categoryRepo.Seed(MakeCategory());

        await _sut.Handle(new CreateProductCommand(SellerId: 100, CategoryId: 10, Name: "Laptop", Description: null, Price: 50_000_000m), _ct);

        Assert.NotNull(_productRepo.Added);
        Assert.Equal("Laptop", _productRepo.Added!.Name);
    }

    /// <summary>Ø®Ø·Ø§: Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ù¾ÛŒØ¯Ø§ Ù†Ø´Ø¯ â†’ CategoryNotFoundException</summary>
    [Fact]
    public async Task Create_When_CategoryNotFound_Should_Throw_CategoryNotFoundException()
    {
        await Assert.ThrowsAsync<CategoryNotFoundException>(
            () => _sut.Handle(new CreateProductCommand(SellerId: 100, CategoryId: 10, Name: "Laptop", Description: null, Price: 50_000_000m), _ct));

        Assert.Null(_productRepo.Added);
    }

    // â”€â”€â”€ UpdateProductCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø¢Ù¾Ø¯ÛŒØª Ù…Ø­ØµÙˆÙ„ Ø¨Ø§ÛŒØ¯ Ø§Ø·Ù„Ø§Ø¹Ø§Øª Ø±Ø§ ØªØºÛŒÛŒØ± Ø¯Ø§Ø¯Ù‡ Ùˆ Repository Ø±Ø§ Ø¨Ø±ÙˆØ²Ø±Ø³Ø§Ù†ÛŒ Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Update_When_ProductAndCategoryExist_Should_UpdateAndSave()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);
        _categoryRepo.Seed(new CategoryAgg(20, "Phones"));

        await _sut.Handle(new UpdateProductCommand(product.Id, "New Name", "New Desc", 20), _ct);

        Assert.Equal("New Name", product.Name);
        Assert.Equal("New Desc", product.Description);
        Assert.Equal(20, product.CategoryId);
    }

    /// <summary>Ø®Ø·Ø§: Ù…Ø­ØµÙˆÙ„ Ù¾ÛŒØ¯Ø§ Ù†Ø´Ø¯ â†’ ProductNotFoundException</summary>
    [Fact]
    public async Task Update_When_ProductNotFound_Should_Throw_ProductNotFoundException()
    {
        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.Handle(new UpdateProductCommand(9999, "Name", null, 10), _ct));
    }

    // â”€â”€â”€ ActivateProductCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>ÙØ¹Ø§Ù„â€ŒØ³Ø§Ø²ÛŒ Ù…Ø­ØµÙˆÙ„ ØºÛŒØ±ÙØ¹Ø§Ù„ Ø¨Ø§ÛŒØ¯ ÙˆØ¶Ø¹ÛŒØª Ø±Ø§ Active Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Activate_When_ProductInactive_Should_ActivateProduct()
    {
        var product = MakeProduct();
        product.Deactivate();
        _productRepo.Seed(product);

        await _sut.Handle(new ActivateProductCommand(product.Id), _ct);

        Assert.Equal(ProductStatus.Active, product.Status);
    }

    /// <summary>Ø®Ø·Ø§: Ù…Ø­ØµÙˆÙ„ Ù¾ÛŒØ¯Ø§ Ù†Ø´Ø¯ â†’ ProductNotFoundException</summary>
    [Fact]
    public async Task Activate_When_ProductNotFound_Should_Throw_ProductNotFoundException()
    {
        await Assert.ThrowsAsync<ProductNotFoundException>(
            () => _sut.Handle(new ActivateProductCommand(9999), _ct));
    }

    // â”€â”€â”€ DeactivateProductCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>ØºÛŒØ±ÙØ¹Ø§Ù„â€ŒØ³Ø§Ø²ÛŒ Ù…Ø­ØµÙˆÙ„ Active Ø¨Ø§ÛŒØ¯ ÙˆØ¶Ø¹ÛŒØª Ø±Ø§ Inactive Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Deactivate_When_ProductActive_Should_DeactivateProduct()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);

        await _sut.Handle(new DeactivateProductCommand(product.Id), _ct);

        Assert.Equal(ProductStatus.Inactive, product.Status);
    }

    // â”€â”€â”€ ArchiveProductCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø¨Ø§ÛŒÚ¯Ø§Ù†ÛŒ Ù…Ø­ØµÙˆÙ„ Active Ø¨Ø§ÛŒØ¯ ÙˆØ¶Ø¹ÛŒØª Ø±Ø§ Archived Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Archive_When_ProductActive_Should_ArchiveProduct()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);

        await _sut.Handle(new ArchiveProductCommand(product.Id), _ct);

        Assert.Equal(ProductStatus.Archived, product.Status);
    }

    // â”€â”€â”€ ReserveStockCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø±Ø²Ø±Ùˆ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ø¨Ø§ÛŒØ¯ Ù…Ù‚Ø¯Ø§Ø± Ø±Ø²Ø±Ùˆ Ø±Ø§ Ø§ÙØ²Ø§ÛŒØ´ Ø¯Ù‡Ø¯</summary>
    [Fact]
    public async Task ReserveStock_When_ProductExists_Should_ReserveInventory()
    {
        var product = MakeProduct();
        product.ReplenishStock(100);
        _productRepo.Seed(product);

        await _sut.Handle(new ReserveStockCommand(product.Id, 30), _ct);

        Assert.Equal(30, product.Inventory.ReservedQuantity);
    }

    // â”€â”€â”€ AddProductImageCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø§ÙØ²ÙˆØ¯Ù† ØªØµÙˆÛŒØ± Ø¨Ø§ÛŒØ¯ ØªØµÙˆÛŒØ± Ø±Ø§ Ø¨Ù‡ Ù…Ø­ØµÙˆÙ„ Ø§Ø¶Ø§ÙÙ‡ Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task AddImage_When_ProductExists_Should_AddImageToProduct()
    {
        var product = MakeProduct();
        _productRepo.Seed(product);

        await _sut.Handle(
            new AddProductImageCommand(product.Id, "https://img.com/1.jpg", "bucket/key", ImageType.Gallery), _ct);

        Assert.Single(product.Images);
    }

    // â”€â”€â”€ CreateCategoryCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø§ÛŒØ¬Ø§Ø¯ Ø¯Ø³ØªÙ‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¬Ø¯ÛŒØ¯ Ø¨Ø§ÛŒØ¯ Ø¢Ù† Ø±Ø§ Ø¯Ø± Repository Ø°Ø®ÛŒØ±Ù‡ Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task CreateCategory_When_NameNotExists_Should_AddCategory()
    {
        _categoryRepo.NameExists = false;

        await _sut.Handle(new CreateCategoryCommand("Electronics", null), _ct);

        Assert.NotNull(_categoryRepo.Added);
        Assert.Equal("Electronics", _categoryRepo.Added!.Name);
    }

    /// <summary>خطا: نام دسته‌بندی تکراری → CategoryNameAlreadyExistsException</summary>
    [Fact]
    public async Task CreateCategory_When_NameExists_Should_Throw_CategoryNameAlreadyExistsException()
    {
        _categoryRepo.NameExists = true;

        await Assert.ThrowsAsync<CategoryNameAlreadyExistsException>(
            () => _sut.Handle(new CreateCategoryCommand("Electronics", null), _ct));
    }
}

