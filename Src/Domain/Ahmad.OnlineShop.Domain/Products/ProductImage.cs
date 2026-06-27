using Ahmad.OnlineShop.Domain.Products.Args;
using Ahmad.OnlineShop.Domain.Products.Enums;
using Ahmad.OnlineShop.Domain.Products.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Products;

public sealed class ProductImage : TEntity<Guid>
{
    public long ProductId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string BucketKey { get; private set; } = string.Empty;
    public ImageType Type { get; private set; }
    public int SortOrder { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private ProductImage() { }

    private ProductImage(CreateProductImageArg arg)
    {
        ProductId = arg.ProductId;
        Url = arg.Url;
        BucketKey = arg.BucketKey;
        Type = arg.Type;
        SortOrder = arg.SortOrder;
        UploadedAt = DateTime.UtcNow;
    }

    public static ProductImage Create(CreateProductImageArg arg)
    {
        GuardValidUrl(arg.Url);
        GuardValidBucketKey(arg.BucketKey);
        GuardValidSortOrder(arg.SortOrder);

        return new ProductImage(arg);
    }

    public void Reorder(int newSortOrder)
    {
        GuardValidSortOrder(newSortOrder);
        SortOrder = newSortOrder;
    }

    public void ChangeType(ImageType newType) => Type = newType;


    private static void GuardValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ImageInvalidUrlException();
    }

    private static void GuardValidBucketKey(string bucketKey)
    {
        if (string.IsNullOrWhiteSpace(bucketKey))
            throw new ImageInvalidBucketKeyException();
    }

    private static void GuardValidSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ImageInvalidSortOrderException();
    }
}