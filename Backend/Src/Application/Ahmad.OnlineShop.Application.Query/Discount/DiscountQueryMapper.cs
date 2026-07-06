using Ahmad.OnlineShop.Application.Query.Queries.Discount;
using Ahmad.OnlineShop.Domain.Discount.Entities;
using DiscountAggregate = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;
using ProductPackage    = Ahmad.OnlineShop.Domain.Discount.Aggregates.ProductPackage;

namespace Ahmad.OnlineShop.Application.Query.Mappers.Discount;

internal static class DiscountQueryMapper
{
    internal static GetDiscountQueryResponse ToResponse(this DiscountAggregate d) => new(
        Id:             d.Id,
        Code:           d.Code,
        Title:          d.Title,
        Type:           d.Type,
        Value:          d.Value,
        MinOrderAmount: d.MinOrderAmount,
        MaxUsage:       d.MaxUsage,
        UsageCount:     d.UsageCount,
        ExpiresAt:      d.ExpiresAt,
        IsActive:       d.IsActive,
        CreatedAt:      d.CreatedAt);

    internal static GetPackageQueryResponse ToResponse(this ProductPackage p) => new(  // ProductPackage alias
        Id:              p.Id,
        Title:           p.Title,
        Description:     p.Description,
        DiscountPercent: p.DiscountPercent,
        ValidFrom:       p.ValidFrom,
        ValidTo:         p.ValidTo,
        IsActive:        p.IsActive,
        CreatedAt:       p.CreatedAt,
        Items:           p.Items.Select(i => new PackageItemDto(i.ProductId, i.Quantity)).ToList());
}
