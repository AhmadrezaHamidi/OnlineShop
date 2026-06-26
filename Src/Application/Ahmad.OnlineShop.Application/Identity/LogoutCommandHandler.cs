using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
{
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepo)
    {
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<bool> Handle(LogoutCommand command, CancellationToken token)
    {
        var refreshToken = await _refreshTokenRepo.GetByTokenAsync(command.RefreshToken, token);
        if (refreshToken is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.InvalidToken);
            throw new UserDomainException(code, msg);
        }

        await _refreshTokenRepo.DeleteAsync(refreshToken, token);
        return true;
    }
}
