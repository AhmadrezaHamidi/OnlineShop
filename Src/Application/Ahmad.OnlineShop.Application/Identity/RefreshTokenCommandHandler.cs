using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Application.Dtos;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, TokenResponseDto>
{
    private readonly IUserRepository         _userRepo;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IJwtService             _jwtService;

    public RefreshTokenCommandHandler(
        IUserRepository         userRepo,
        IRefreshTokenRepository refreshTokenRepo,
        IJwtService             jwtService)
    {
        _userRepo         = userRepo;
        _refreshTokenRepo = refreshTokenRepo;
        _jwtService       = jwtService;
    }

    public async Task<TokenResponseDto> Handle(RefreshTokenCommand command, CancellationToken token)
    {
        var storedToken = await _refreshTokenRepo.GetByTokenAsync(command.RefreshToken, token);
        if (storedToken is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.InvalidToken);
            throw new UserDomainException(code, msg);
        }

        storedToken.EnsureValid();

        var user = await _userRepo.GetByIdAsync(storedToken.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        // Rotate: delete old, issue new
        await _refreshTokenRepo.DeleteAsync(storedToken, token);

        var (accessToken, accessExpiry) = _jwtService.GenerateAccessToken(user);
        var rawRefreshToken             = _jwtService.GenerateRefreshToken();
        var refreshExpiry               = DateTime.UtcNow.AddDays(30);

        var newId              = await _refreshTokenRepo.GetNextIdAsync();
        var newRefreshToken    = new RefreshToken(newId, user.Id, rawRefreshToken, refreshExpiry);
        await _refreshTokenRepo.AddAsync(newRefreshToken, token);

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
