using AhmadBase.Application;

namespace Identity.Application.Commands;

public record AssignRoleCommand(
    long UserId,
    long RoleId
) : ICommand<bool>;
