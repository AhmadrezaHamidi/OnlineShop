using AhmadBase.Application;

namespace Identity.Application.Commands;

public record LogoutCommand(
    string RefreshToken
) : ICommand<bool>;
