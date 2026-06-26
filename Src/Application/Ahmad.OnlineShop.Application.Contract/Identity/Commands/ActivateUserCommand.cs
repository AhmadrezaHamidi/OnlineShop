using AhmadBase.Application;

namespace Identity.Application.Commands;

public record ActivateUserCommand(long UserId) : ICommand<bool>;
