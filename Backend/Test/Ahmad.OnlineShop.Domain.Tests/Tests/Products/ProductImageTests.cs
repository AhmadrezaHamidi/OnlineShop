/// <summary>
/// تست‌های Entity تصویر محصول (ProductImage)
/// پوشش‌دهنده: ایجاد، تغییر ترتیب، تغییر نوع تصویر
/// خطاهای تست‌شده: ImageInvalidUrlException | ImageInvalidBucketKeyException | ImageInvalidSortOrderException
/// </summary>
namespace Ahmad.OnlineShop.Domain.Products.Tests;

public class ProductImageTests
{
    private static CreateProductImageArg GalleryArg() =>
        new(Guid.NewGuid(), 1, "https://cdn.example.com/img.jpg", "bucket/key", ImageType.Gallery, 0);

    // ── Create ───────────────────────────────────────────────────────────────

    /// <summary>ایجاد تصویر باید مشخصات را ست کند</summary>
    [Fact]
    public void Create_Should_Set_All_Properties()
    {
        var image = ProductImage.Create(GalleryArg());

        Assert.Equal("https://cdn.example.com/img.jpg", image.Url);
        Assert.Equal("bucket/key",      image.BucketKey);
        Assert.Equal(ImageType.Gallery, image.Type);
        Assert.Equal(0,                 image.SortOrder);
    }

    /// <summary>خطا: URL خالی → ImageInvalidUrlException</summary>
    [Fact]
    public void Create_When_EmptyUrl_Should_Throw_ImageInvalidUrlException()
    {
        var arg = new CreateProductImageArg(Guid.NewGuid(), 1, "", "key", ImageType.Gallery, 0);

        Assert.Throws<ImageInvalidUrlException>(() => ProductImage.Create(arg));
    }

    /// <summary>خطا: URL فضای خالی → ImageInvalidUrlException</summary>
    [Fact]
    public void Create_When_WhitespaceUrl_Should_Throw_ImageInvalidUrlException()
    {
        var arg = new CreateProductImageArg(Guid.NewGuid(), 1, "   ", "key", ImageType.Gallery, 0);

        Assert.Throws<ImageInvalidUrlException>(() => ProductImage.Create(arg));
    }

    /// <summary>خطا: BucketKey خالی → ImageInvalidBucketKeyException</summary>
    [Fact]
    public void Create_When_EmptyBucketKey_Should_Throw_ImageInvalidBucketKeyException()
    {
        var arg = new CreateProductImageArg(Guid.NewGuid(), 1, "https://url.com", "", ImageType.Gallery, 0);

        Assert.Throws<ImageInvalidBucketKeyException>(() => ProductImage.Create(arg));
    }

    /// <summary>خطا: ترتیب نمایش منفی → ImageInvalidSortOrderException</summary>
    [Fact]
    public void Create_When_NegativeSortOrder_Should_Throw_ImageInvalidSortOrderException()
    {
        var arg = new CreateProductImageArg(Guid.NewGuid(), 1, "https://url.com", "key", ImageType.Gallery, -1);

        Assert.Throws<ImageInvalidSortOrderException>(() => ProductImage.Create(arg));
    }

    // ── Reorder ──────────────────────────────────────────────────────────────

    /// <summary>تغییر ترتیب باید SortOrder را آپدیت کند</summary>
    [Fact]
    public void Reorder_Should_Update_SortOrder()
    {
        var image = ProductImage.Create(GalleryArg());
        image.Reorder(5);

        Assert.Equal(5, image.SortOrder);
    }

    /// <summary>خطا: ترتیب منفی → ImageInvalidSortOrderException</summary>
    [Fact]
    public void Reorder_When_NegativeSortOrder_Should_Throw_ImageInvalidSortOrderException()
    {
        var image = ProductImage.Create(GalleryArg());

        Assert.Throws<ImageInvalidSortOrderException>(() => image.Reorder(-1));
    }

    /// <summary>تغییر ترتیب به صفر باید موفق باشد</summary>
    [Fact]
    public void Reorder_To_Zero_Should_Succeed()
    {
        var image = ProductImage.Create(GalleryArg());
        image.Reorder(0);

        Assert.Equal(0, image.SortOrder);
    }

    // ── ChangeType ───────────────────────────────────────────────────────────

    /// <summary>تغییر نوع تصویر باید Type را آپدیت کند</summary>
    [Fact]
    public void ChangeType_Should_Update_ImageType()
    {
        var image = ProductImage.Create(GalleryArg());
        image.ChangeType(ImageType.Primary);

        Assert.Equal(ImageType.Primary, image.Type);
    }
}
