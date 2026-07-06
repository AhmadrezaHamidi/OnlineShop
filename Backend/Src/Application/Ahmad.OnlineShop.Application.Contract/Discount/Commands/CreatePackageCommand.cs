namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record CreatePackageCommand(
    string   Title,
    string?  Description,
    decimal  DiscountPercent,
    DateTime ValidFrom,
    DateTime ValidTo
) : ICommand<long>;
