namespace Ahmad.OnlineShop.Domain.Products.Tests;

public class ProductTests
{
    private static CreateProductArg ValidArg() =>
        new(Id: 1, CategoryId: 10, Name: "Laptop", Description: "Gaming Laptop", Price: 50_000_000m, InventoryId: 1);

    private static Product CreateProduct() => Product.Create(ValidArg());

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_Should_Set_Properties_And_ActiveStatus()
    {
        var product = CreateProduct();

        Assert.Equal(10,               product.CategoryId);
        Assert.Equal("Laptop",         product.Name);
        Assert.Equal(50_000_000m,      product.Price);
        Assert.Equal(ProductStatus.Active, product.Status);
        Assert.NotNull(product.Inventory);
        Assert.Empty(product.Images);
    }

    [Fact]
    public void Create_When_EmptyName_Should_Throw()
    {
        var arg = new CreateProductArg(1, 10, "", null, 50_000m, 1);

        Assert.Throws<EmptyProductNameException>(() => Product.Create(arg));
    }

    [Fact]
    public void Create_When_WhitespaceName_Should_Throw()
    {
        var arg = new CreateProductArg(1, 10, "   ", null, 50_000m, 1);

        Assert.Throws<EmptyProductNameException>(() => Product.Create(arg));
    }

    [Fact]
    public void Create_When_ZeroPrice_Should_Throw()
    {
        var arg = new CreateProductArg(1, 10, "Laptop", null, 0, 1);

        Assert.Throws<InvalidPriceException>(() => Product.Create(arg));
    }

    [Fact]
    public void Create_When_NegativePrice_Should_Throw()
    {
        var arg = new CreateProductArg(1, 10, "Laptop", null, -1000m, 1);

        Assert.Throws<InvalidPriceException>(() => Product.Create(arg));
    }

    // ── UpdateDetails ────────────────────────────────────────────────────────

    [Fact]
    public void UpdateDetails_Should_Update_Name_Description_Category()
    {
        var product = CreateProduct();

        product.UpdateDetails("New Name", "New Desc", 20);

        Assert.Equal("New Name", product.Name);
        Assert.Equal("New Desc", product.Description);
        Assert.Equal(20,          product.CategoryId);
    }

    [Fact]
    public void UpdateDetails_When_EmptyName_Should_Throw()
    {
        var product = CreateProduct();

        Assert.Throws<EmptyProductNameException>(() => product.UpdateDetails("", null, 10));
    }

    // ── ChangePrice ──────────────────────────────────────────────────────────

    [Fact]
    public void ChangePrice_Should_Update_Price()
    {
        var product = CreateProduct();

        product.ChangePrice(60_000_000m);

        Assert.Equal(60_000_000m, product.Price);
    }

    [Fact]
    public void ChangePrice_When_ZeroPrice_Should_Throw()
    {
        var product = CreateProduct();

        Assert.Throws<InvalidPriceException>(() => product.ChangePrice(0));
    }

    // ── Activate / Deactivate / Archive ──────────────────────────────────────

    [Fact]
    public void Deactivate_Should_Change_Status_To_Inactive()
    {
        var product = CreateProduct();

        product.Deactivate();

        Assert.Equal(ProductStatus.Inactive, product.Status);
    }

    [Fact]
    public void Activate_After_Deactivate_Should_Change_Status_To_Active()
    {
        var product = CreateProduct();
        product.Deactivate();

        product.Activate();

        Assert.Equal(ProductStatus.Active, product.Status);
    }

    [Fact]
    public void Activate_When_AlreadyActive_Should_Throw()
    {
        var product = CreateProduct();

        Assert.Throws<ProductAlreadyArchivedException>(() => product.Activate());
    }

    [Fact]
    public void Deactivate_When_AlreadyInactive_Should_Throw()
    {
        var product = CreateProduct();
        product.Deactivate();

        Assert.Throws<ProductAlreadyInactiveException>(() => product.Deactivate());
    }

    [Fact]
    public void Archive_Should_Change_Status_To_Archived()
    {
        var product = CreateProduct();

        product.Archive();

        Assert.Equal(ProductStatus.Archived, product.Status);
    }

    [Fact]
    public void Archive_When_AlreadyArchived_Should_Throw()
    {
        var product = CreateProduct();
        product.Archive();

        Assert.Throws<ProductAlreadyArchivedException>(() => product.Archive());
    }

    // ── Stock ─────────────────────────────────────────────────────────────────

    [Fact]
    public void ReserveStock_Should_Reserve_Inventory()
    {
        var product = CreateProduct();
        product.ReplenishStock(100);

        product.ReserveStock(30);

        Assert.Equal(30, product.Inventory.ReservedQuantity);
        Assert.Equal(70, product.Inventory.AvailableQuantity);
    }

    [Fact]
    public void ReleaseStock_Should_Release_Reserved_Inventory()
    {
        var product = CreateProduct();
        product.ReplenishStock(100);
        product.ReserveStock(50);

        product.ReleaseStock(20);

        Assert.Equal(30, product.Inventory.ReservedQuantity);
    }

    [Fact]
    public void ReplenishStock_Should_Add_To_Inventory()
    {
        var product = CreateProduct();

        product.ReplenishStock(100);

        Assert.Equal(100, product.Inventory.Quantity);
    }

    [Fact]
    public void ConfirmStock_Should_Deduct_From_Inventory()
    {
        var product = CreateProduct();
        product.ReplenishStock(100);
        product.ReserveStock(30);

        product.ConfirmStock(30);

        Assert.Equal(70, product.Inventory.Quantity);
        Assert.Equal(0,  product.Inventory.ReservedQuantity);
    }

    // ── Images ────────────────────────────────────────────────────────────────

    [Fact]
    public void AddImage_Should_Add_To_Images_Collection()
    {
        var product = CreateProduct();
        var arg     = new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/1.jpg", "key1", ImageType.Gallery, 0);

        product.AddImage(arg);

        Assert.Single(product.Images);
    }

    [Fact]
    public void AddImage_Primary_When_PrimaryExists_Should_Throw()
    {
        var product = CreateProduct();
        var arg1    = new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/1.jpg", "key1", ImageType.Primary, 0);
        var arg2    = new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/2.jpg", "key2", ImageType.Primary, 1);

        product.AddImage(arg1);

        Assert.Throws<PrimaryImageExistsException>(() => product.AddImage(arg2));
    }

    [Fact]
    public void AddImage_When_MaxImagesExceeded_Should_Throw()
    {
        var product = CreateProduct();

        for (var i = 0; i < 10; i++)
            product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, $"https://img.com/{i}.jpg", $"key{i}", ImageType.Gallery, i));

        Assert.Throws<MaxImagesExceededException>(
            () => product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/11.jpg", "key11", ImageType.Gallery, 11)));
    }

    [Fact]
    public void RemoveImage_Should_Remove_From_Images()
    {
        var product = CreateProduct();
        var arg     = new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/1.jpg", "key1", ImageType.Gallery, 0);
        var image   = product.AddImage(arg);

        product.RemoveImage(image.Id);

        Assert.Empty(product.Images);
    }

    [Fact]
    public void RemoveImage_When_NotFound_Should_Throw()
    {
        var product = CreateProduct();

        Assert.Throws<ImageNotFoundException>(() => product.RemoveImage(Guid.NewGuid()));
    }

    [Fact]
    public void SetPrimaryImage_Should_Change_Target_To_Primary_And_Others_To_Gallery()
    {
        var product = CreateProduct();
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/1.jpg", "k1", ImageType.Gallery, 0));
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/2.jpg", "k2", ImageType.Gallery, 1));

        var target  = product.Images.First();
        product.SetPrimaryImage(target.Id);

        Assert.Equal(ImageType.Primary, product.Images.First().Type);
        Assert.All(product.Images.Skip(1), img => Assert.Equal(ImageType.Gallery, img.Type));
    }
}
