using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record SetPrimaryImageCommand(
    long ProductId,
    Guid ImageId
) : ICommand<Guid>;
