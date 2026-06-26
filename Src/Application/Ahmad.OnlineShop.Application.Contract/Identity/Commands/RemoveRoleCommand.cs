using AhmadBase.Application;

namespace Identity.Application.Commands;

public record RemoveRoleCommand(
    long UserId,
    long RoleId
) : ICommand<bool>;
