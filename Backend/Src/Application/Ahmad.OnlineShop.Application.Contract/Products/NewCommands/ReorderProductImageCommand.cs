using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReorderProductImageCommand(
    [property: JsonIgnore] long ProductId,
    [property: JsonIgnore] Guid ImageId,
    int NewSortOrder
) : ICommand<Guid>;
