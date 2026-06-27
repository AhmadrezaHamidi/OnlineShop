using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Identity.Application.Commands;

public record RemoveRoleCommand(
    [property: JsonIgnore] long UserId,
    [property: JsonIgnore] long RoleId
) : ICommand<bool>;
