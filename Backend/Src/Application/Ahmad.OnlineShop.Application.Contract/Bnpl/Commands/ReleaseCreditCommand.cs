using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record ReleaseCreditCommand(
    [property: JsonIgnore] long UserId,
    decimal Amount
) : ICommand<long>;
