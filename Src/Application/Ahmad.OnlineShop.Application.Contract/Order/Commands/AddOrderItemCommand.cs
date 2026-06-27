using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record AddOrderItemCommand(
    [property: JsonIgnore] long OrderId,
    long    ProductId,
    int     Quantity,
    decimal UnitPrice
) : ICommand<long>;
