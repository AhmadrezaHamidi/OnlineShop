using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Identity.Application.Commands;

public record SuspendUserCommand(
    [property: JsonIgnore] long UserId
) : ICommand<bool>;
