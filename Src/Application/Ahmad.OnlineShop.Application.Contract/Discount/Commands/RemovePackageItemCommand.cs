namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record RemovePackageItemCommand(
    long PackageId,
    long ProductId
) : ICommand<bool>;
