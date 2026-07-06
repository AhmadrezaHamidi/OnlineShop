using Ahmad.OnlineShop.Domain.Products.Enums;
using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetProductQuery(long Id) : IQuery<GetProductQueryResponse>;

public sealed record GetProductQueryResponse(
    long Id,
    long CategoryId,
    string Name,
    string? Description,
    decimal Price,
    ProductStatus Status,
    DateTimeOffset CreatedAt,
    GetProductInventoryResponse Inventory,
    List<GetProductImageResponse> Images);

public sealed record GetProductInventoryResponse(
    long ProductId,
    int Quantity,
    int ReservedQuantity,
    int AvailableQuantity,
    bool IsLowStock,
    bool IsOutOfStock);

public sealed record GetProductImageResponse(
    Guid Id,
    string Url,
    ImageType Type,
    int SortOrder,
    DateTime UploadedAt);
