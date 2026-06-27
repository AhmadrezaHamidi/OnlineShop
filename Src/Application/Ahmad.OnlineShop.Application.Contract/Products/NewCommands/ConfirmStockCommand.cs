using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record ConfirmStockCommand(
    [property: JsonIgnore] long ProductId,
    int Quantity
) : ICommand<long>;
