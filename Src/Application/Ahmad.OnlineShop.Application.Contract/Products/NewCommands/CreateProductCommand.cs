using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record CreateProductCommand(
    long     CategoryId,
    string   Name,
    string?  Description,
    decimal  Price
) : ICommand<long>;
