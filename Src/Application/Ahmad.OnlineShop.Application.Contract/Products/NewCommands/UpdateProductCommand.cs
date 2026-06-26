using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record UpdateProductCommand(
    long    Id,
    string  Name,
    string? Description,
    long    CategoryId
) : ICommand<long>;
