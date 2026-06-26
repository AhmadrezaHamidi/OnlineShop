namespace Ahmad.OnlineShop.Application.Dtos;

public record InventoryDto(
    long ProductId,
    int  Quantity,
    int  ReservedQuantity,
    int  AvailableQuantity,
    bool IsLowStock,
    bool IsOutOfStock
);
