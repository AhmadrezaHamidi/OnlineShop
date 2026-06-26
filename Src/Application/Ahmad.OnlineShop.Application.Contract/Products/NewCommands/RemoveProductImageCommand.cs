using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record RemoveProductImageCommand(
    long ProductId,
    Guid ImageId
) : ICommand<Guid>;
