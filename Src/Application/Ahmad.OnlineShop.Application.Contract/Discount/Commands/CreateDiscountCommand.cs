using Ahmad.OnlineShop.Domain.Discount.Enums;

namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record CreateDiscountCommand(
    string       Code,
    string       Title,
    DiscountType Type,
    decimal      Value,
    decimal?     MinOrderAmount,
    int?         MaxUsage,
    DateTime?    ExpiresAt
) : ICommand<long>;
