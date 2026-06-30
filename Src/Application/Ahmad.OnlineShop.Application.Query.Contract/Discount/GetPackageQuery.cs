namespace Ahmad.OnlineShop.Application.Query.Queries.Discount;

public sealed record GetPackageQuery(long PackageId) : IQuery<GetPackageQueryResponse?>;

public sealed record GetPackagesQuery(
    int   Page     = 1,
    int   PageSize = 20,
    bool? IsActive = null
) : IQuery<PagedResult<GetPackageQueryResponse>>;

public sealed record GetPackageQueryResponse(
    long                          Id,
    string                        Title,
    string?                       Description,
    decimal                       DiscountPercent,
    DateTime                      ValidFrom,
    DateTime                      ValidTo,
    bool                          IsActive,
    DateTime                      CreatedAt,
    IReadOnlyList<PackageItemDto> Items
);

public sealed record PackageItemDto(
    long ProductId,
    int  Quantity
);
