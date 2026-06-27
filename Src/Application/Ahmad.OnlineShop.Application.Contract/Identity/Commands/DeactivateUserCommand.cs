using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Identity.Application.Commands;

public record DeactivateUserCommand(
    [property: JsonIgnore] long UserId
) : ICommand<bool>;
