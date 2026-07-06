namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record ActivatePackageCommand(long PackageId) : ICommand<bool>;
