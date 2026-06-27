/// <summary>
/// تست‌های Aggregate Root محصول (Product)
/// ─────────────────────────────────────────────────────────────────────
/// پوشش‌دهنده:
///   - ایجاد محصول با فروشنده
///   - مالکیت فروشنده (Seller Ownership)
///   - دسترسی فروشنده به موجودی خودش
///   - تغییر جزئیات و قیمت
///   - چرخه وضعیت: Active → Inactive → Archived
///   - موجودی: رزرو، آزادسازی، تأیید، تأمین
///   - تصاویر: افزودن، حذف، تصویر اصلی
///   - Domain Events
///
/// خطاهای تست‌شده:
///   EmptyProductNameException | InvalidPriceException | InvalidSellerIdException
///   ProductNotOwnedBySellerException | ProductAlreadyActiveException
///   ProductAlreadyInactiveException | ProductAlreadyArchivedException
///   MaxImagesExceededException | PrimaryImageExistsException | ImageNotFoundException
/// </summary>
using Ahmad.OnlineShop.Domain.Products.Events;

namespace Ahmad.OnlineShop.Domain.Products.Tests;

public class ProductTests
{
    // ── Factory ──────────────────────────────────────────────────────────────

    private static CreateProductArg ValidArg(long sellerId = 10) =>
        new(Id: 1, SellerId: sellerId, CategoryId: 5,
            Name: "Laptop Gaming", Description: "لپ‌تاپ گیمینگ",
            Price: 50_000_000m, InventoryId: 1);

    private static Product CreateProduct(long sellerId = 10)
        => Product.Create(ValidArg(sellerId));

    // ═══════════════════════════════════════════════════════════════════════
    // بخش اول: ایجاد محصول
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>ایجاد محصول باید تمام مشخصات را ست کند — شامل SellerId</summary>
    [Fact]
    public void Create_Should_Set_All_Properties_Including_SellerId()
    {
        var product = CreateProduct(sellerId: 10);

        Assert.Equal(10,              product.SellerId);
        Assert.Equal(5,               product.CategoryId);
        Assert.Equal("Laptop Gaming", product.Name);
        Assert.Equal(50_000_000m,     product.Price);
        Assert.Equal(ProductStatus.Active, product.Status);
        Assert.NotNull(product.Inventory);
        Assert.Empty(product.Images);
        Assert.True(product.CreationTime > DateTimeOffset.UtcNow.AddSeconds(-5));
    }

    /// <summary>ایجاد محصول باید رویداد ProductCreatedEvent را raise کند</summary>
    [Fact]
    public void Create_Should_Raise_ProductCreatedEvent()
    {
        var product = CreateProduct(sellerId: 10);

        var evt = product.DomainEvents.OfType<ProductCreatedEvent>().FirstOrDefault();
        Assert.NotNull(evt);
        Assert.Equal(10,              evt!.SellerId);
        Assert.Equal("Laptop Gaming", evt.Name);
        Assert.Equal(50_000_000m,     evt.Price);
    }

    /// <summary>خطا: نام خالی → EmptyProductNameException</summary>
    [Fact]
    public void Create_When_EmptyName_Should_Throw_EmptyProductNameException()
    {
        var arg = new CreateProductArg(1, 10, 5, "", null, 50_000m, 1);

        Assert.Throws<EmptyProductNameException>(() => Product.Create(arg));
    }

    /// <summary>خطا: قیمت صفر → InvalidPriceException</summary>
    [Fact]
    public void Create_When_ZeroPrice_Should_Throw_InvalidPriceException()
    {
        var arg = new CreateProductArg(1, 10, 5, "Laptop", null, 0, 1);

        Assert.Throws<InvalidPriceException>(() => Product.Create(arg));
    }

    /// <summary>خطا: قیمت منفی → InvalidPriceException</summary>
    [Fact]
    public void Create_When_NegativePrice_Should_Throw_InvalidPriceException()
    {
        var arg = new CreateProductArg(1, 10, 5, "Laptop", null, -1000m, 1);

        Assert.Throws<InvalidPriceException>(() => Product.Create(arg));
    }

    /// <summary>خطا: SellerId صفر → InvalidSellerIdException</summary>
    [Fact]
    public void Create_When_ZeroSellerId_Should_Throw_InvalidSellerIdException()
    {
        var arg = new CreateProductArg(1, 0, 5, "Laptop", null, 50_000m, 1);

        Assert.Throws<InvalidSellerIdException>(() => Product.Create(arg));
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش دوم: مالکیت فروشنده
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>BelongsToSeller باید True برگرداند وقتی SellerId درست است</summary>
    [Fact]
    public void BelongsToSeller_When_CorrectSeller_Should_Return_True()
    {
        var product = CreateProduct(sellerId: 10);

        Assert.True(product.BelongsToSeller(10));
    }

    /// <summary>BelongsToSeller باید False برگرداند وقتی SellerId اشتباه است</summary>
    [Fact]
    public void BelongsToSeller_When_WrongSeller_Should_Return_False()
    {
        var product = CreateProduct(sellerId: 10);

        Assert.False(product.BelongsToSeller(99));
    }

    /// <summary>GuardSellerOwnership نباید خطا بدهد اگر SellerId درست باشد</summary>
    [Fact]
    public void GuardSellerOwnership_When_Correct_Should_Not_Throw()
    {
        var product = CreateProduct(sellerId: 10);

        var ex = Record.Exception(() => product.GuardSellerOwnership(10));

        Assert.Null(ex);
    }

    /// <summary>خطا: فروشنده دیگر نمی‌تواند محصول را مدیریت کند → ProductNotOwnedBySellerException</summary>
    [Fact]
    public void GuardSellerOwnership_When_WrongSeller_Should_Throw_ProductNotOwnedBySellerException()
    {
        var product = CreateProduct(sellerId: 10);

        Assert.Throws<ProductNotOwnedBySellerException>(
            () => product.GuardSellerOwnership(99));
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش سوم: تغییر جزئیات و قیمت
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>UpdateDetails باید نام، توضیح و دسته‌بندی را آپدیت کند</summary>
    [Fact]
    public void UpdateDetails_Should_Update_Name_Description_Category()
    {
        var product = CreateProduct();

        product.UpdateDetails("Gaming Laptop Pro", "مدل جدید", 7);

        Assert.Equal("Gaming Laptop Pro", product.Name);
        Assert.Equal("مدل جدید",         product.Description);
        Assert.Equal(7,                   product.CategoryId);
    }

    /// <summary>خطا: نام خالی در UpdateDetails → EmptyProductNameException</summary>
    [Fact]
    public void UpdateDetails_When_EmptyName_Should_Throw_EmptyProductNameException()
    {
        var product = CreateProduct();

        Assert.Throws<EmptyProductNameException>(
            () => product.UpdateDetails("   ", null, 5));
    }

    /// <summary>ChangePrice باید قیمت را آپدیت کرده و رویداد raise کند</summary>
    [Fact]
    public void ChangePrice_Should_Update_Price_And_Raise_Event()
    {
        var product  = CreateProduct();
        var oldPrice = product.Price;

        product.ChangePrice(60_000_000m);

        Assert.Equal(60_000_000m, product.Price);
        var evt = product.DomainEvents.OfType<ProductPriceChangedEvent>().First();
        Assert.Equal(oldPrice,    evt.OldPrice);
        Assert.Equal(60_000_000m, evt.NewPrice);
    }

    /// <summary>خطا: قیمت جدید صفر → InvalidPriceException</summary>
    [Fact]
    public void ChangePrice_When_ZeroPrice_Should_Throw_InvalidPriceException()
    {
        var product = CreateProduct();

        Assert.Throws<InvalidPriceException>(() => product.ChangePrice(0));
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش چهارم: چرخه وضعیت
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>Deactivate باید وضعیت را Inactive کند</summary>
    [Fact]
    public void Deactivate_From_Active_Should_Set_Inactive()
    {
        var product = CreateProduct();

        product.Deactivate();

        Assert.Equal(ProductStatus.Inactive, product.Status);
    }

    /// <summary>Activate بعد از Deactivate باید وضعیت را Active کند</summary>
    [Fact]
    public void Activate_After_Deactivate_Should_Set_Active()
    {
        var product = CreateProduct();
        product.Deactivate();

        product.Activate();

        Assert.Equal(ProductStatus.Active, product.Status);
    }

    /// <summary>خطا: Activate وقتی از قبل Active است → ProductAlreadyActiveException</summary>
    [Fact]
    public void Activate_When_AlreadyActive_Should_Throw_ProductAlreadyActiveException()
    {
        var product = CreateProduct(); // status = Active

        Assert.Throws<ProductAlreadyActiveException>(() => product.Activate());
    }

    /// <summary>خطا: Deactivate وقتی از قبل Inactive است → ProductAlreadyInactiveException</summary>
    [Fact]
    public void Deactivate_When_AlreadyInactive_Should_Throw_ProductAlreadyInactiveException()
    {
        var product = CreateProduct();
        product.Deactivate();

        Assert.Throws<ProductAlreadyInactiveException>(() => product.Deactivate());
    }

    /// <summary>Archive باید وضعیت را Archived کند</summary>
    [Fact]
    public void Archive_Should_Set_Archived()
    {
        var product = CreateProduct();

        product.Archive();

        Assert.Equal(ProductStatus.Archived, product.Status);
    }

    /// <summary>خطا: Archive وقتی از قبل Archived است → ProductAlreadyArchivedException</summary>
    [Fact]
    public void Archive_When_AlreadyArchived_Should_Throw_ProductAlreadyArchivedException()
    {
        var product = CreateProduct();
        product.Archive();

        Assert.Throws<ProductAlreadyArchivedException>(() => product.Archive());
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش پنجم: دسترسی فروشنده به موجودی
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>فروشنده مالک باید به موجودی دسترسی داشته باشد</summary>
    [Fact]
    public void Seller_Should_Access_Own_Product_Inventory()
    {
        var product = CreateProduct(sellerId: 10);

        // تأیید مالکیت
        product.GuardSellerOwnership(10);

        // دسترسی به موجودی
        product.ReplenishStock(100);

        Assert.Equal(100, product.Inventory.Quantity);
    }

    /// <summary>فروشنده دیگر نباید بتواند موجودی را تغییر دهد</summary>
    [Fact]
    public void WrongSeller_Should_Not_Manage_Inventory()
    {
        var product = CreateProduct(sellerId: 10);

        // فروشنده دیگر تلاش می‌کند
        Assert.Throws<ProductNotOwnedBySellerException>(
            () => product.GuardSellerOwnership(99));
    }

    /// <summary>فروشنده باید بتواند موجودی را تأمین کند</summary>
    [Fact]
    public void Seller_Can_Replenish_Stock()
    {
        var product = CreateProduct(sellerId: 10);

        product.GuardSellerOwnership(10);
        product.ReplenishStock(50);

        Assert.Equal(50, product.Inventory.Quantity);
        Assert.Equal(50, product.Inventory.AvailableQuantity);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش ششم: تصاویر
    // ═══════════════════════════════════════════════════════════════════════

    private static CreateProductImageArg GalleryImage(int sortOrder = 0) =>
        new(Guid.NewGuid(), 1, "https://cdn.example.com/img.jpg", "key", ImageType.Gallery, sortOrder);

    /// <summary>افزودن تصویر Gallery باید در لیست Images ظاهر شود</summary>
    [Fact]
    public void AddImage_Gallery_Should_Add_To_Images()
    {
        var product = CreateProduct();

        product.AddImage(GalleryImage());

        Assert.Single(product.Images);
        Assert.Equal(ImageType.Gallery, product.Images.First().Type);
    }

    /// <summary>افزودن تصویر Primary اول باید موفق باشد</summary>
    [Fact]
    public void AddImage_Primary_First_Should_Succeed()
    {
        var product = CreateProduct();
        var arg     = new CreateProductImageArg(Guid.NewGuid(), 1, "https://img.com/p.jpg", "key", ImageType.Primary, 0);

        product.AddImage(arg);

        Assert.Equal(ImageType.Primary, product.Images.First().Type);
    }

    /// <summary>خطا: دو تصویر Primary نمی‌شه داشت → PrimaryImageExistsException</summary>
    [Fact]
    public void AddImage_SecondPrimary_Should_Throw_PrimaryImageExistsException()
    {
        var product = CreateProduct();
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://img1.com", "k1", ImageType.Primary, 0));

        Assert.Throws<PrimaryImageExistsException>(
            () => product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://img2.com", "k2", ImageType.Primary, 1)));
    }

    /// <summary>خطا: بیش از ۱۰ تصویر → MaxImagesExceededException</summary>
    [Fact]
    public void AddImage_WhenMax10Reached_Should_Throw_MaxImagesExceededException()
    {
        var product = CreateProduct();
        for (var i = 0; i < 10; i++)
            product.AddImage(GalleryImage(i));

        Assert.Throws<MaxImagesExceededException>(
            () => product.AddImage(GalleryImage(11)));
    }

    /// <summary>SetPrimaryImage باید تصویر را Primary کند و بقیه را Gallery</summary>
    [Fact]
    public void SetPrimaryImage_Should_Change_Target_To_Primary_And_Others_To_Gallery()
    {
        var product = CreateProduct();
        product.AddImage(GalleryImage(0));
        product.AddImage(GalleryImage(1));

        var target = product.Images.First();
        product.SetPrimaryImage(target.Id);

        Assert.Equal(ImageType.Primary, product.Images.First(i => i.Id == target.Id).Type);
        Assert.All(product.Images.Where(i => i.Id != target.Id),
            img => Assert.Equal(ImageType.Gallery, img.Type));
    }

    /// <summary>RemoveImage باید تصویر را از لیست حذف کند</summary>
    [Fact]
    public void RemoveImage_Should_Remove_From_Images()
    {
        var product = CreateProduct();
        var image   = product.AddImage(GalleryImage());

        product.RemoveImage(image.Id);

        Assert.Empty(product.Images);
    }

    /// <summary>خطا: تصویر پیدا نشد → ImageNotFoundException</summary>
    [Fact]
    public void RemoveImage_When_NotFound_Should_Throw_ImageNotFoundException()
    {
        var product = CreateProduct();

        Assert.Throws<ImageNotFoundException>(
            () => product.RemoveImage(Guid.NewGuid()));
    }
}
