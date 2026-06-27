using AhmadBase.Application;

namespace Identity.Application.Commands;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<LoginCommandResponse>;

public sealed record LoginCommandResponse(
    string   AccessToken,
    string   RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    long     UserId,
    string   FullName,
    string   Email);
