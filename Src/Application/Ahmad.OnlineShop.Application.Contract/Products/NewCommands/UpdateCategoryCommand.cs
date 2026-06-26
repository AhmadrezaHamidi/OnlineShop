using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record UpdateCategoryCommand(
    long   Id,
    string Name,
    long?  ParentId
) : ICommand<long>;
