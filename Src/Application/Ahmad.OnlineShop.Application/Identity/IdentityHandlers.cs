using Identity.Application.Commands;
using Identity.Application.Services;
using Identity.Domain.Aggregates;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public sealed class IdentityHandlers(
    IUserRepository userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    IRoleRepository roleRepo,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) :
    ICommandHandler<RegisterCommand, long>,
    ICommandHandler<LoginCommand, LoginCommandResponse>,
    ICommandHandler<LogoutCommand, bool>,
    ICommandHandler<RefreshTokenCommand, LoginCommandResponse>,
    ICommandHandler<ChangePasswordCommand, bool>,
    ICommandHandler<UpdateProfileCommand, bool>,
    ICommandHandler<ActivateUserCommand, bool>,
    ICommandHandler<DeactivateUserCommand, bool>,
    ICommandHandler<SuspendUserCommand, bool>,
    ICommandHandler<AssignRoleCommand, bool>,
    ICommandHandler<RemoveRoleCommand, bool>
{
    #region Auth Commands

    public async Task<long> Handle(RegisterCommand command, CancellationToken token)
    {
        var exists = await userRepo.ExistsByEmailAsync(command.Email, token);
        if (exists)
            throw new UserAlreadyExistsException();

        var id = await userRepo.GetNextIdAsync();
        var passwordHash = passwordHasher.Hash(command.Password);
        var user = User.Register(id, command.FullName, command.Email, passwordHash, command.PhoneNumber);

        await userRepo.AddAsync(user, token);

        return user.Id;
    }

    public async Task<LoginCommandResponse> Handle(LoginCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByEmailAsync(command.Email, token)
            ?? throw new UserNotFoundException();

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new IncorrectPasswordException();

        var (accessToken, accessExpiry) = jwtService.GenerateAccessToken(user);
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(30);

        var refreshTokenId = await refreshTokenRepo.GetNextIdAsync();
        var refreshTokenEntity = new RefreshToken(refreshTokenId, user.Id, rawRefreshToken, refreshExpiry);
        await refreshTokenRepo.AddAsync(refreshTokenEntity, token);

        return new LoginCommandResponse(
            AccessToken: accessToken,
            RefreshToken: rawRefreshToken,
            AccessTokenExpiresAt: accessExpiry,
            RefreshTokenExpiresAt: refreshExpiry,
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email);
    }

    public async Task<bool> Handle(LogoutCommand command, CancellationToken token)
    {
        var refreshToken = await refreshTokenRepo.GetByTokenAsync(command.RefreshToken, token)
            ?? throw new InvalidRefreshTokenException();

        await refreshTokenRepo.DeleteAsync(refreshToken, token);
        return true;
    }

    public async Task<LoginCommandResponse> Handle(RefreshTokenCommand command, CancellationToken token)
    {
        var storedToken = await refreshTokenRepo.GetByTokenAsync(command.RefreshToken, token)
            ?? throw new InvalidRefreshTokenException();

        storedToken.EnsureValid();

        var user = await userRepo.GetByIdAsync(storedToken.UserId, token)
            ?? throw new UserNotFoundException();

        await refreshTokenRepo.DeleteAsync(storedToken, token);

        var (accessToken, accessExpiry) = jwtService.GenerateAccessToken(user);
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(30);

        var newId = await refreshTokenRepo.GetNextIdAsync();
        var newRefreshToken = new RefreshToken(newId, user.Id, rawRefreshToken, refreshExpiry);
        await refreshTokenRepo.AddAsync(newRefreshToken, token);

        return new LoginCommandResponse(
            AccessToken: accessToken,
            RefreshToken: rawRefreshToken,
            AccessTokenExpiresAt: accessExpiry,
            RefreshTokenExpiresAt: refreshExpiry,
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email);
    }

    #endregion

    #region User Profile Commands

    public async Task<bool> Handle(ChangePasswordCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            throw new IncorrectPasswordException();

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));

        await userRepo.UpdateAsync(user, token);
        return true;
    }

    public async Task<bool> Handle(UpdateProfileCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        user.UpdateProfile(command.FullName, command.PhoneNumber);

        await userRepo.UpdateAsync(user, token);
        return true;
    }

    #endregion

    #region User Status Commands

    public async Task<bool> Handle(ActivateUserCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        user.Activate();

        await userRepo.UpdateAsync(user, token);
        return true;
    }

    public async Task<bool> Handle(DeactivateUserCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        user.Deactivate();

        await userRepo.UpdateAsync(user, token);
        return true;
    }

    public async Task<bool> Handle(SuspendUserCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        user.Suspend();

        await userRepo.UpdateAsync(user, token);
        return true;
    }

    #endregion

    #region Role Commands

    public async Task<bool> Handle(AssignRoleCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        _ = await roleRepo.GetByIdAsync(command.RoleId, token)
            ?? throw new RoleNotFoundException();

        user.AssignRole(command.RoleId);

        await userRepo.UpdateAsync(user, token);
        return true;
    }

    public async Task<bool> Handle(RemoveRoleCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        _ = await roleRepo.GetByIdAsync(command.RoleId, token)
            ?? throw new RoleNotFoundException();

        user.RemoveRole(command.RoleId);

        await userRepo.UpdateAsync(user, token);
        return true;
    }

    #endregion
}
