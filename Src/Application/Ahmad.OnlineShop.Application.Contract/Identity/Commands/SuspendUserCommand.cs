using AhmadBase.Application;

namespace Identity.Application.Commands;

public record SuspendUserCommand(long UserId) : ICommand<bool>;
