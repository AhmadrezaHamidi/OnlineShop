namespace Ahmad.OnlineShop.Application.Dtos;

public sealed record OrderItemDto(
    long    Id,
    long    ProductId,
    int     Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
