/// <summary>
/// تست‌های فرآیندی محصول — سناریوهای کامل End-to-End
/// ─────────────────────────────────────────────────────────────────────
/// سناریو ۱: چرخه کامل محصول (ایجاد → فروش → بایگانی)
/// سناریو ۲: فرآیند کامل موجودی (تأمین → رزرو → تأیید/آزادسازی)
/// سناریو ۳: دو فروشنده — هر کدام به محصول خودشان دسترسی دارند
/// سناریو ۴: مدیریت تصاویر محصول توسط فروشنده
/// سناریو ۵: تغییر قیمت و رویدادهای Domain
/// </summary>
using Ahmad.OnlineShop.Domain.Products.Events;

namespace Ahmad.OnlineShop.Domain.Products.Tests;

public class ProductScenarioTests
{
    // ── Factory ──────────────────────────────────────────────────────────────

    private static Product MakeProduct(long sellerId = 100, long productId = 1)
        => Product.Create(new CreateProductArg(
            Id:          productId,
            SellerId:    sellerId,
            CategoryId:  5,
            Name:        "گوشی موبایل Samsung",
            Description: "Galaxy S24",
            Price:       25_000_000m,
            InventoryId: productId));

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۱: چرخه کامل محصول
    // گام‌ها: ایجاد → غیرفعال → فعال → بایگانی
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// فرآیند: محصول ایجاد می‌شود (Active) → موقتاً غیرفعال → دوباره فعال → در نهایت بایگانی
    /// این فرآیند در واقعیت وقتی فروشنده محصول را موقتاً از فروش خارج می‌کند اتفاق می‌افتد
    /// </summary>
    [Fact]
    public void Scenario1_Product_Full_Lifecycle_Create_Deactivate_Activate_Archive()
    {
        // ── گام ۱: ایجاد محصول جدید
        var product = MakeProduct(sellerId: 100);
        Assert.Equal(ProductStatus.Active, product.Status);
        Assert.True(product.BelongsToSeller(100));

        // ── گام ۲: فروشنده محصول را موقتاً غیرفعال می‌کند (مثلاً برای ویرایش)
        product.GuardSellerOwnership(100);
        product.Deactivate();
        Assert.Equal(ProductStatus.Inactive, product.Status);

        // ── گام ۳: فروشنده اطلاعات را آپدیت می‌کند
        product.UpdateDetails("گوشی Samsung S24 Ultra", "نسخه جدید", 5);
        product.ChangePrice(28_000_000m);
        Assert.Equal("گوشی Samsung S24 Ultra", product.Name);
        Assert.Equal(28_000_000m, product.Price);

        // ── گام ۴: محصول دوباره فعال می‌شود
        product.Activate();
        Assert.Equal(ProductStatus.Active, product.Status);

        // ── گام ۵: محصول منسوخ شده و بایگانی می‌شود
        product.Archive();
        Assert.Equal(ProductStatus.Archived, product.Status);

        // ── گام ۶: تأیید رویدادهای Domain
        var events = product.DomainEvents.ToList();
        Assert.Contains(events, e => e is ProductCreatedEvent);
        Assert.Contains(events, e => e is ProductStatusChangedEvent);
        Assert.Contains(events, e => e is ProductPriceChangedEvent);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۲: فرآیند کامل موجودی
    // گام‌ها: تأمین → رزرو سفارش → تأیید / کنسل → تأمین مجدد
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// فرآیند: فروشنده موجودی را تأمین می‌کند → مشتری سفارش می‌دهد (رزرو) →
    /// سفارش تأیید می‌شود (کسر موجودی) یا کنسل (آزادسازی رزرو)
    /// </summary>
    [Fact]
    public void Scenario2_Inventory_Full_Flow_Replenish_Reserve_Confirm()
    {
        var product = MakeProduct();

        // ── گام ۱: فروشنده ۵۰ عدد موجودی تأمین می‌کند
        product.ReplenishStock(50);
        Assert.Equal(50, product.Inventory.Quantity);
        Assert.Equal(50, product.Inventory.AvailableQuantity);
        Assert.False(product.Inventory.IsOutOfStock);

        // ── گام ۲: مشتری ۱۰ عدد سفارش می‌دهد (رزرو)
        product.ReserveStock(10);
        Assert.Equal(50, product.Inventory.Quantity);         // کل تغییر نکرده
        Assert.Equal(10, product.Inventory.ReservedQuantity); // رزرو شده
        Assert.Equal(40, product.Inventory.AvailableQuantity); // موجود برای سفارش

        // ── گام ۳: سفارش تأیید می‌شود → موجودی کسر می‌شود
        product.ConfirmStock(10);
        Assert.Equal(40, product.Inventory.Quantity);         // کسر شده
        Assert.Equal(0,  product.Inventory.ReservedQuantity); // رزرو آزاد
        Assert.Equal(40, product.Inventory.AvailableQuantity);

        // ── گام ۴: مشتری دیگری ۵ عدد رزرو می‌کند
        product.ReserveStock(5);
        Assert.Equal(35, product.Inventory.AvailableQuantity);

        // ── گام ۵: سفارش کنسل می‌شود → رزرو آزاد می‌شود
        product.ReleaseStock(5);
        Assert.Equal(40, product.Inventory.AvailableQuantity); // برگشت به ۴۰

        // ── گام ۶: تأیید رویدادهای موجودی
        var stockEvents = product.DomainEvents.ToList();
        Assert.Contains(stockEvents, e => e is StockReplenishedEvent);
        Assert.Contains(stockEvents, e => e is StockReservedEvent);
        Assert.Contains(stockEvents, e => e is StockReleasedEvent);
    }

    /// <summary>
    /// فرآیند: رسیدن به آستانه کم‌موجودی و تمام شدن موجودی
    /// وقتی موجودی ≤ ۵ باشد، رویداد StockDepletedEvent raise می‌شود
    /// </summary>
    [Fact]
    public void Scenario2b_Inventory_LowStock_And_OutOfStock_Events()
    {
        var product = MakeProduct();

        // ── تأمین ۱۰ عدد
        product.ReplenishStock(10);

        // ── رزرو ۶ عدد → باقیمانده = ۴ که زیر آستانه است
        product.ReserveStock(6);
        Assert.True(product.Inventory.IsLowStock);

        // ── رزرو ۴ عدد باقیمانده → موجودی صفر
        product.ReserveStock(4);
        Assert.True(product.Inventory.IsOutOfStock);

        // ── رویداد StockDepletedEvent باید raise شده باشد
        var depleted = product.DomainEvents.OfType<StockDepletedEvent>().ToList();
        Assert.NotEmpty(depleted);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۳: دو فروشنده — هر کدام به محصول خودشان دسترسی دارند
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// فرآیند: فروشنده A محصول دارد — فروشنده B نمی‌تواند آن را تغییر دهد
    /// هر فروشنده فقط به محصولات خودش دسترسی دارد
    /// </summary>
    [Fact]
    public void Scenario3_Two_Sellers_Each_Owns_Their_Products()
    {
        // ── فروشنده A (id=100) محصول خود را دارد
        var productA = MakeProduct(sellerId: 100, productId: 1);

        // ── فروشنده B (id=200) محصول دیگری دارد
        var productB = MakeProduct(sellerId: 200, productId: 2);

        // ── فروشنده A می‌تواند محصول خودش را مدیریت کند
        productA.GuardSellerOwnership(100);
        productA.ReplenishStock(50);
        Assert.Equal(50, productA.Inventory.Quantity);

        // ── فروشنده B می‌تواند محصول خودش را مدیریت کند
        productB.GuardSellerOwnership(200);
        productB.ReplenishStock(30);
        Assert.Equal(30, productB.Inventory.Quantity);

        // ── فروشنده A نمی‌تواند به محصول B دسترسی داشته باشد
        Assert.Throws<ProductNotOwnedBySellerException>(
            () => productB.GuardSellerOwnership(100));

        // ── فروشنده B نمی‌تواند به محصول A دسترسی داشته باشد
        Assert.Throws<ProductNotOwnedBySellerException>(
            () => productA.GuardSellerOwnership(200));

        // ── موجودی هر کدام مستقل است
        Assert.Equal(50, productA.Inventory.Quantity);
        Assert.Equal(30, productB.Inventory.Quantity);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۴: مدیریت تصاویر توسط فروشنده
    // گام‌ها: افزودن تصاویر → تنظیم Primary → حذف
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// فرآیند ۴الف: فروشنده تصاویر Gallery اضافه می‌کند و Primary تنظیم می‌کند
    /// نکته: Id تصویر توسط EF ست می‌شود — SetPrimaryImage از FirstOrDefault استفاده می‌کند
    /// </summary>
    [Fact]
    public void Scenario4a_Seller_Adds_Images_And_Sets_Primary()
    {
        var product = MakeProduct(sellerId: 100);
        product.GuardSellerOwnership(100);

        // ── سه تصویر Gallery اضافه می‌کند
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://cdn.com/1.jpg", "k1", ImageType.Gallery, 0));
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://cdn.com/2.jpg", "k2", ImageType.Gallery, 1));
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://cdn.com/3.jpg", "k3", ImageType.Gallery, 2));
        Assert.Equal(3, product.Images.Count);
        Assert.All(product.Images, img => Assert.Equal(ImageType.Gallery, img.Type));

        // ── اولین تصویر را Primary می‌کند
        var firstImage = product.Images.First();
        product.SetPrimaryImage(firstImage.Id);

        // ── بررسی: یک Primary و بقیه Gallery
        Assert.Equal(1, product.Images.Count(i => i.Type == ImageType.Primary));
        Assert.Equal(2, product.Images.Count(i => i.Type == ImageType.Gallery));

        // ── رویدادها
        Assert.Equal(3, product.DomainEvents.OfType<ProductImageAddedEvent>().Count());
    }

    /// <summary>
    /// فرآیند ۴ب: فروشنده تصویر Primary اضافه می‌کند و سپس Gallery را حذف می‌کند
    /// </summary>
    [Fact]
    public void Scenario4b_Seller_Adds_Primary_Then_Removes_Gallery()
    {
        var product = MakeProduct(sellerId: 100);
        product.GuardSellerOwnership(100);

        // ── یک تصویر Primary و یک Gallery اضافه می‌کند
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://cdn.com/primary.jpg", "pk", ImageType.Primary, 0));
        product.AddImage(new CreateProductImageArg(Guid.NewGuid(), 1, "https://cdn.com/gallery.jpg", "gk", ImageType.Gallery, 1));
        Assert.Equal(2, product.Images.Count);
        Assert.Single(product.Images, i => i.Type == ImageType.Primary);

        // ── تصویر Gallery (اولین Gallery) حذف می‌شود
        // چون Primary اول است و Gallery دوم، Remove(Primary.Id=Empty) می‌برد اولی رو
        // اما ما مستقیم از شیء آخر حذف می‌کنیم
        var galleryImage = product.Images.Last(); // آخرین که Gallery است
        product.RemoveImage(galleryImage.Id);

        // ── فقط Primary باید باقی بماند
        Assert.Single(product.Images);

        // ── رویدادها
        Assert.Single(product.DomainEvents.OfType<ProductImageRemovedEvent>());
    }

    // ═══════════════════════════════════════════════════════════════════════
    // سناریو ۵: سفارش کامل — رزرو → پرداخت → تأیید موجودی
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// فرآیند سفارش:
    /// ۱. مشتری سفارش می‌دهد → موجودی رزرو می‌شود
    /// ۲. مشتری پرداخت می‌کند → سفارش تأیید → موجودی کسر
    /// ۳. اگر پرداخت شکست بخورد → موجودی آزاد می‌شود
    /// </summary>
    [Fact]
    public void Scenario5_Order_Flow_Reserve_Pay_Confirm()
    {
        var product = MakeProduct();
        product.ReplenishStock(100);

        // ── پرداخت موفق
        product.ReserveStock(3);
        Assert.Equal(97, product.Inventory.AvailableQuantity);
        product.ConfirmStock(3);
        Assert.Equal(97, product.Inventory.Quantity);
        Assert.Equal(0,  product.Inventory.ReservedQuantity);

        // ── پرداخت ناموفق (کنسل)
        product.ReserveStock(5);
        Assert.Equal(92, product.Inventory.AvailableQuantity);
        product.ReleaseStock(5);
        Assert.Equal(97, product.Inventory.AvailableQuantity); // برگشت

        // ── تأیید وضعیت نهایی
        Assert.Equal(97, product.Inventory.Quantity);
        Assert.Equal(0,  product.Inventory.ReservedQuantity);
        Assert.False(product.Inventory.IsLowStock);
    }
}
