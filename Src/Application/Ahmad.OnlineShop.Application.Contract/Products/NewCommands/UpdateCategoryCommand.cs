using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record UpdateCategoryCommand(
    [property: JsonIgnore] long Id,
    string Name,
    long?  ParentId
) : ICommand<long>;
