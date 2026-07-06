using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record UpdateProductCommand(
    [property: JsonIgnore] long Id,
    string  Name,
    string? Description,
    long    CategoryId
) : ICommand<long>;
