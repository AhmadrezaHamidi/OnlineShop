using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ChangeProductPriceCommand(
    long    Id,
    decimal NewPrice
) : ICommand<long>;
