using AhmadBase.Application;

namespace Identity.Application.Commands;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : ICommand<LoginCommandResponse>;
