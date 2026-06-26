using AhmadBase.Application;
using Identity.Application.Dtos;

namespace Identity.Application.Commands;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<TokenResponseDto>;
