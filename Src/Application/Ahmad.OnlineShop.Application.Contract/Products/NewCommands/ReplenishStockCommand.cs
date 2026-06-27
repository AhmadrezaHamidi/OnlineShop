using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReplenishStockCommand(
    [property: JsonIgnore] long ProductId,
    int Quantity
) : ICommand<long>;
