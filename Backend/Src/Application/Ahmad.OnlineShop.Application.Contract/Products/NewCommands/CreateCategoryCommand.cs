using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record CreateCategoryCommand(
    string Name,
    long?  ParentId
) : ICommand<long>;
