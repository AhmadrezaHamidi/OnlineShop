using AhmadBase.Application;

namespace Identity.Application.Commands;

public record DeactivateUserCommand(long UserId) : ICommand<bool>;
