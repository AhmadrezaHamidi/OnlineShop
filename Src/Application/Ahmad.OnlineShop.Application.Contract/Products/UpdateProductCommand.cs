using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Products
;

public sealed record UpdateProductCommand(
        long Id,
        string Name,
        string Description,
        long CategoryId
    ) : ICommand<long>;
