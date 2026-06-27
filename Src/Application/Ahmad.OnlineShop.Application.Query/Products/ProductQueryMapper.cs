using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Products;

namespace Ahmad.OnlineShop.Application.Query.Mappers;

internal static class ProductQueryMapper
{
    internal static GetProductQueryResponse ToResponse(this Product product)
        => new(
            product.Id,
            product.CategoryId,
            product.Name,
            product.Description,
            product.Price,
            product.Status,
            product.CreationTime,
            product.Inventory.ToResponse(),
            product.Images.Select(i => i.ToResponse()).ToList());

    internal static GetProductInventoryResponse ToResponse(this Inventory inv)
        => new(inv.ProductId, inv.Quantity, inv.ReservedQuantity, inv.AvailableQuantity, inv.IsLowStock, inv.IsOutOfStock);

    internal static GetProductImageResponse ToResponse(this ProductImage i)
        => new(i.Id, i.Url, i.Type, i.SortOrder, i.UploadedAt);

    internal static GetCategoryQueryResponse ToResponse(this Category c)
        => new(c.Id, c.Name, c.ParentId);
}
