using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Application.Dtos;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Aggregates;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public class LoginCommandHandler : ICommandHandler<LoginCommand, TokenResponseDto>
{
    private readonly IUserRepository         _userRepo;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IPasswordHasher         _passwordHasher;
    private readonly IJwtService             _jwtService;

    public LoginCommandHandler(
        IUserRepository         userRepo,
        IRefreshTokenRepository refreshTokenRepo,
        IPasswordHasher         passwordHasher,
        IJwtService             jwtService)
    {
        _userRepo         = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _passwordHasher   = passwordHasher;
        _jwtService       = jwtService;
    }

    public async Task<TokenResponseDto> Handle(LoginCommand command, CancellationToken token)
    {
        var user = await _userRepo.GetByEmailAsync(command.Email, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        var valid = _passwordHasher.Verify(command.Password, user.PasswordHash);
        if (!valid)
        {
            var (code, msg) = UserErrors.Get(UserErrors.InvalidPasswordHash);
            throw new UserDomainException(code, msg);
        }

        var (accessToken, accessExpiry) = _jwtService.GenerateAccessToken(user);
        var rawRefreshToken             = _jwtService.GenerateRefreshToken();
        var refreshExpiry               = DateTime.UtcNow.AddDays(30);

        var refreshTokenId    = await _refreshTokenRepo.GetNextIdAsync();
        var refreshTokenEntity = new RefreshToken(refreshTokenId, user.Id, rawRefreshToken, refreshExpiry);
        await _refreshTokenRepo.AddAsync(refreshTokenEntity, token);

        return new TokenResponseDto(
            AccessToken:            accessToken,
            RefreshToken:           rawRefreshToken,
            AccessTokenExpiresAt:   accessExpiry,
            RefreshTokenExpiresAt:  refreshExpiry,
            UserId:                 user.Id,
            FullName:               user.FullName,
            Email:                  user.Email);
    }
}
