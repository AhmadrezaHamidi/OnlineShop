using Ahmad.OnlineShop.Domain.Discount.Enums;

namespace Ahmad.OnlineShop.Domain.Discount.Args;

public record CreateDiscountArg(
    long          Id,
    string        Code,
    string        Title,
    DiscountType  Type,
    decimal       Value,
    decimal?      MinOrderAmount,
    int?          MaxUsage,
    DateTime?     ExpiresAt
);

public record CreatePackageArg(
    long     Id,
    string   Title,
    string?  Description,
    decimal  DiscountPercent,
    DateTime ValidFrom,
    DateTime ValidTo
);
