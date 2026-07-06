using Ahmad.OnlineShop.Domain.Products.Enums;

namespace Ahmad.OnlineShop.Domain.Products.Args;

public sealed record CreateProductArg(
    long     Id,
    long     SellerId,
    long     CategoryId,
    string   Name,
    string?  Description,
    decimal  Price,
    long     InventoryId
);

public sealed record CreateCategoryArg(
    long    Id,
    string  Name,
    long?   ParentId = null
);

public sealed record CreateInventoryArg(
    long Id,
    long ProductId,
    int  InitialQuantity = 0
);

public sealed record CreateProductImageArg(
    Guid      Id,
    long      ProductId,
    string    Url,
    string    BucketKey,
    ImageType Type      = ImageType.Gallery,
    int       SortOrder = 0
);
