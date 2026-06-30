using Ahmad.OnlineShop.Persistence.EF;
using Ahmad.OnlineShop.Persistence.EF.Services;
using Identity.Application.Commands;
using Identity.Application.Services;
using Identity.Domain.Aggregates;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

/// <summary>
/// Handler های Identity با احراز هویت OTP
/// - RequestOtp: تولید کد 6 رقمی و ارسال SMS
/// - VerifyOtp:  تأیید کد و صدور JWT (اگر کاربر جدید باشد، خودکار ثبت می‌شود)
/// - RefreshToken: تجدید توکن
/// - Logout: باطل کردن refresh token
/// </summary>
public sealed class IdentityHandlers(
    IUserRepository userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    IRoleRepository roleRepo,
    IOtpRepository otpRepo,
    ISmsService smsService,
    IJwtService jwtService,
    IdentityAppDbContext identityDb) :
    ICommandHandler<RequestOtpCommand, bool>,
    ICommandHandler<VerifyOtpCommand, LoginCommandResponse>,
    ICommandHandler<RefreshTokenCommand, LoginCommandResponse>,
    ICommandHandler<LogoutCommand, bool>,
    ICommandHandler<UpdateProfileCommand, bool>,
    ICommandHandler<ActivateUserCommand, bool>,
    ICommandHandler<DeactivateUserCommand, bool>,
    ICommandHandler<SuspendUserCommand, bool>,
    ICommandHandler<AssignRoleCommand, bool>,
    ICommandHandler<RemoveRoleCommand, bool>
{
    #region OTP Auth

    public async Task<bool> Handle(RequestOtpCommand command, CancellationToken token)
    {
        var code = OtpCodeGenerator.Generate();
        var id = await otpRepo.GetNextIdAsync();
        var otp = new OtpRequest(id, command.PhoneNumber, code);

        await otpRepo.AddAsync(otp, token);
        await identityDb.SaveChangesAsync(token);

        return await smsService.SendOtpAsync(command.PhoneNumber, code, token);
    }

    public async Task<LoginCommandResponse> Handle(VerifyOtpCommand command, CancellationToken token)
    {
        var otp = await otpRepo.GetLatestByPhoneAsync(command.PhoneNumber, token)
            ?? throw new OtpNotRequestedException();

        otp.Verify(command.Code);
        await otpRepo.UpdateAsync(otp, token);

        var user = await userRepo.GetByPhoneAsync(command.PhoneNumber, token);
        if (user is null)
        {
            var id = await userRepo.GetNextIdAsync();
            user = User.Create(id, command.PhoneNumber);
            await userRepo.AddAsync(user, token);
        }

        var result = await IssueTokens(user, token);
        await identityDb.SaveChangesAsync(token);
        return result;
    }

    #endregion

    #region Token Management

    public async Task<LoginCommandResponse> Handle(RefreshTokenCommand command, CancellationToken token)
    {
        var storedToken = await refreshTokenRepo.GetByTokenAsync(command.RefreshToken, token)
            ?? throw new InvalidRefreshTokenException();

        storedToken.EnsureValid();

        var user = await userRepo.GetByIdAsync(storedToken.UserId, token)
            ?? throw new UserNotFoundException();

        await refreshTokenRepo.DeleteAsync(storedToken, token);

        var result = await IssueTokens(user, token);
        await identityDb.SaveChangesAsync(token);
        return result;
    }

    public async Task<bool> Handle(LogoutCommand command, CancellationToken token)
    {
        var refreshToken = await refreshTokenRepo.GetByTokenAsync(command.RefreshToken, token)
            ?? throw new InvalidRefreshTokenException();

        await refreshTokenRepo.DeleteAsync(refreshToken, token);
        await identityDb.SaveChangesAsync(token);
        return true;
    }

    #endregion

    #region Profile

    public async Task<bool> Handle(UpdateProfileCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        user.UpdateProfile(command.FullName);
        await userRepo.UpdateAsync(user, token);
        await identityDb.SaveChangesAsync(token);
        return true;
    }

    #endregion

    #region User Status

    public async Task<bool> Handle(ActivateUserCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();
        user.Activate();
        await userRepo.UpdateAsync(user, token);
        await identityDb.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> Handle(DeactivateUserCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();
        user.Deactivate();
        await userRepo.UpdateAsync(user, token);
        await identityDb.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> Handle(SuspendUserCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();
        user.Suspend();
        await userRepo.UpdateAsync(user, token);
        await identityDb.SaveChangesAsync(token);
        return true;
    }

    #endregion

    #region Roles

    public async Task<bool> Handle(AssignRoleCommand command, CancellationToken token)
    {
        var user = await userRepo.GetByIdAsync(command.UserId, token)
            ?? throw new UserNotFoundException();

        _ = await roleRepo.GetByIdAsync(command.RoleId, token)
            ?? throw new RoleNotFoundException();

        user.AssignRole(command.RoleId);
        await userRepo.UpdateAsync(user, token);
        await identityDb.SaveChangesAsync(token);
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
        await identityDb.SaveChangesAsync(token);
        return true;
    }

    #endregion

    #region Helpers

    private async Task<LoginCommandResponse> IssueTokens(User user, CancellationToken token)
    {
        var jwtResult = jwtService.GenerateAccessToken(user);
        var rawRefresh = jwtService.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(30);
        var newId = await refreshTokenRepo.GetNextIdAsync();

        await refreshTokenRepo.AddAsync(
            new RefreshToken(newId, user.Id, rawRefresh, refreshExpiry), token);

        return new LoginCommandResponse(
            AccessToken: jwtResult.Token,
            RefreshToken: rawRefresh,
            AccessTokenExpiresAt: jwtResult.ExpiresAt,
            RefreshTokenExpiresAt: refreshExpiry,
            UserId: user.Id,
            FullName: user.FullName ?? string.Empty,
            Email: user.PhoneNumber);
    }

    #endregion
}
