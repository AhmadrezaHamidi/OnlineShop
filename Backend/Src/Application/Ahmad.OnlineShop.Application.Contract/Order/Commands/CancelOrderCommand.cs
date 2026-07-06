using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record CancelOrderCommand(
    [property: JsonIgnore] long OrderId,
    string Reason
) : ICommand<long>;
