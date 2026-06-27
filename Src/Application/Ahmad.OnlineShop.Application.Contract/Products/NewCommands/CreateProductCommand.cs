using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record CreateProductCommand(
    [property: JsonIgnore] long SellerId,
    long     CategoryId,
    string   Name,
    string?  Description,
    decimal  Price
) : ICommand<long>;
