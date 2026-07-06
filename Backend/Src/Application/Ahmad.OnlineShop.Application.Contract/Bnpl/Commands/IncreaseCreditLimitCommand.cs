using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Commands;

public record IncreaseCreditLimitCommand(
    [property: JsonIgnore] long UserId,
    decimal NewLimit
) : ICommand<long>;
