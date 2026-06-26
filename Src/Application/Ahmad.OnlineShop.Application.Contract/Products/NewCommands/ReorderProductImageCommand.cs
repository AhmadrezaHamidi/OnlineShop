using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReorderProductImageCommand(
    long ProductId,
    Guid ImageId,
    int  NewSortOrder
) : ICommand<Guid>;
