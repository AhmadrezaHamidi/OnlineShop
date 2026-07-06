using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Discount.Entities;

public sealed class PackageItem : TEntity<long>
{
    public long PackageId  { get; private set; }
    public long ProductId  { get; private set; }
    public int  Quantity   { get; private set; }

    private PackageItem() { }

    internal PackageItem(long id, long packageId, long productId, int quantity)
    {
        Id        = id;
        PackageId = packageId;
        ProductId = productId;
        Quantity  = quantity;
    }
}
