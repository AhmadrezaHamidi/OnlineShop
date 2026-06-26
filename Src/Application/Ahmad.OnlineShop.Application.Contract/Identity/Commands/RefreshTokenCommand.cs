using AhmadBase.Application;
using Identity.Application.Dtos;

namespace Identity.Application.Commands;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : ICommand<TokenResponseDto>;
