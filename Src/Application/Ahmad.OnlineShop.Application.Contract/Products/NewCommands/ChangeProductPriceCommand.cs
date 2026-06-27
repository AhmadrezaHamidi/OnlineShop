using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record ChangeProductPriceCommand(
    [property: JsonIgnore] long Id,
    decimal NewPrice
) : ICommand<long>;
