using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Products
;

public sealed record ChangeProductPriceCommand(
        long Id,
        decimal NewPrice
    ) : ICommand<long>;
