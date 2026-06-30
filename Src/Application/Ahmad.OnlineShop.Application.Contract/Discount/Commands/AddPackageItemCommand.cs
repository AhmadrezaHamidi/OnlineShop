namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record AddPackageItemCommand(
    long PackageId,
    long ProductId,
    int  Quantity
) : ICommand<bool>;
