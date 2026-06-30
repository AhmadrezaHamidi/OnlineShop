namespace Ahmad.OnlineShop.Application.Commands.Discount;

public sealed record DeactivateDiscountCommand(long DiscountId) : ICommand<bool>;
