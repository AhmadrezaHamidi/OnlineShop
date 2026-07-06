using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ActivateProductCommand(
    long Id
) : ICommand<long>;
