namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record DeactivatePackageCommand(long PackageId) : ICommand<bool>;
