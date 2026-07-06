using Ahmad.OnlineShop.Domain.Discount.Enums;

namespace Ahmad.OnlineShop.Application.Query.Queries.Discount;

public sealed record GetDiscountQuery(long DiscountId) : IQuery<GetDiscountQueryResponse?>;

public sealed record GetDiscountByCodeQuery(string Code) : IQuery<GetDiscountQueryResponse?>;

public sealed record GetDiscountsQuery(
    int     Page     = 1,
    int     PageSize = 20,
    bool?   IsActive = null
) : IQuery<PagedResult<GetDiscountQueryResponse>>;

public sealed record GetDiscountQueryResponse(
    long          Id,
    string        Code,
    string        Title,
    DiscountType  Type,
    decimal       Value,
    decimal?      MinOrderAmount,
    int?          MaxUsage,
    int           UsageCount,
    DateTime?     ExpiresAt,
    bool          IsActive,
    DateTime      CreatedAt
);
