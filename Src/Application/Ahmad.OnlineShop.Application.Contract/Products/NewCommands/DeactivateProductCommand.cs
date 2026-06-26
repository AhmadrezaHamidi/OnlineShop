using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record DeactivateProductCommand(
    long Id
) : ICommand<long>;
