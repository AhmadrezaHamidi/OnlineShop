using AhmadBase.Application;

namespace Identity.Application.Commands;

public record ChangePasswordCommand(
    long   UserId,
    string CurrentPassword,
    string NewPassword
) : ICommand<bool>;
