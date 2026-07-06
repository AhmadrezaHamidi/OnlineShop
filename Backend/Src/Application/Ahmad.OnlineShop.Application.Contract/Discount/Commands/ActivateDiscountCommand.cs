namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record ActivateDiscountCommand(long DiscountId) : ICommand<bool>;
