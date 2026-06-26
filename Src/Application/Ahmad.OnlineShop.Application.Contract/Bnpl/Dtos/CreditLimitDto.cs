namespace Ahmad.OnlineShop.Application.Dtos;

public record CreditLimitDto(
    long     UserId,
    decimal  TotalLimit,
    decimal  UsedLimit,
    decimal  AvailableLimit,
    DateTime UpdatedAt
);
