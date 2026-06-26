using Ahmad.OnlineShop.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Dtos;

public record ProductDto(
    long                   Id,
    long                   CategoryId,
    string                 Name,
    string?                Description,
    decimal                Price,
    ProductStatus          Status,
    DateTime               CreatedAt,
    InventoryDto           Inventory,
    List<ProductImageDto>  Images
);
