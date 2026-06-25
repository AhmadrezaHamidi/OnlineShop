using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Products
;

public sealed record DeactivateProductCommand(long Id) : ICommand<long>;
