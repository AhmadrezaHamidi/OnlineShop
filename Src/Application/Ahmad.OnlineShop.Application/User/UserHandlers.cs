using Ahmad.OnlineShop.Application.Contract.User;
using Ahmad.OnlineShop.Application.Contract.Opetions;
using Ahmad.OnlineShop.Application.Exceptions;
using Ahmad.OnlineShop.Domain.Users;
using Ahmad.OnlineShop.Domain.Users.Args;
using Ahmad.OnlineShop.Domain.Users.Exceptions;
using UserAgg = Ahmad.OnlineShop.Domain.User.User;
using AhmadBase.Application;
using AhmadBase.Web.Securities;
using AhmadBase.Helper.Cryptography;

namespace Ahmad.OnlineShop.Application.User;

/// <summary>
/// Handler های کاربر قدیمی (IdentityUser-based)
/// چک تکراری بودن در Application Handler انجام می‌شود — نه در Domain
/// </summary>
public class UserHandlers(
    IUserRepository repository,
    JWTOptions jwtOptions) :
    ICommandHandler<LoginCommand, TokenResponse>,
    ICommandHandler<ChangePasswordCommand, long>,
    ICommandHandler<LogoutCommand, long>,
    ICommandHandler<RefreshTokenCommand, TokenResponse>,
    ICommandHandler<RegisterCommand, long>
{
    public async Task<TokenResponse> Handle(LoginCommand command, CancellationToken token)
    {
        var user = await repository.Get(userName: command.UserName, token);
        if (user is null)
            throw new UserNotFoundException();

        var hashedPassword = PasswordHasher.HashPassword(command.Password, jwtOptions.Secret);
        var sessionId = user.Login(hashedPassword);
        await repository.Update(token);

        Console.WriteLine($"[DEBUG] TokenExpireMinutes={jwtOptions.TokenExpireMinutes} RefreshExpireMinutes={jwtOptions.RefreshExpireMinutes} Secret={jwtOptions.Secret}");

        return JwtHandler.GenerateTokens(new TokenRequest
        {
            UserId = user.Id.ToString(),
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            SessionId = sessionId,
            Roles = user.Roles.Select(r => r.RoleId.ToString()).ToList(),
            ValidIssuer = jwtOptions.ValidIssuer,
            ValidAudience = jwtOptions.ValidAudience,
            TokenExpireMinutes = jwtOptions.TokenExpireMinutes,
            Secret = jwtOptions.Secret
        });
    }

    public async Task<long> Handle(ChangePasswordCommand command, CancellationToken token)
    {
        var user = await repository.Get(command.UserId, token)
            ?? throw new UserNotFoundException();

        if (command.NewPassword != command.ConfirmNewPassword)
            throw new PasswordMismatchException();

        var currentHash = PasswordHasher.HashPassword(command.CurrentPassword, jwtOptions.Secret);
        user.Login(currentHash);
        user.ChangePassword(PasswordHasher.HashPassword(command.NewPassword, jwtOptions.Secret));
        await repository.Update(token);

        return user.Id;
    }

    public async Task<long> Handle(LogoutCommand command, CancellationToken token)
    {
        var user = await repository.Get(command.UserId, token)
            ?? throw new UserNotFoundException();

        user.Logout(user.Sessions.First().SessionId);
        await repository.Update(token);

        return user.Id;
    }

    public async Task<TokenResponse> Handle(RefreshTokenCommand command, CancellationToken token)
    {
        JwtHandler.ValidateRefreshToken(new RefreshTokenRequest
        {
            Token = command.RefreshToken,
            ValidIssuer = jwtOptions.ValidIssuer,
            ValidAudience = jwtOptions.ValidAudience,
            Secret = jwtOptions.Secret
        });

        var result = JwtHandler.GetRefreshResult(command.RefreshToken);
        var user = await repository.Get(result.UserId, token)
            ?? throw new UserNotFoundException();

        user.Logout(result.SessionId);
        var sessionId = user.Login(user.PasswordHash!);
        await repository.Update(token);

        return JwtHandler.GenerateTokens(new TokenRequest
        {
            UserId = user.Id.ToString(),
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            SessionId = sessionId,
            Roles = user.Roles.Select(r => r.RoleId.ToString()).ToList(),
            ValidIssuer = jwtOptions.ValidIssuer,
            ValidAudience = jwtOptions.ValidAudience,
            TokenExpireMinutes = jwtOptions.TokenExpireMinutes,
            Secret = jwtOptions.Secret
        });
    }

    public async Task<long> Handle(RegisterCommand command, CancellationToken token)
    {
        if (command.Password != command.ConfirmPassword)
            throw new PasswordMismatchException();

        // چک تکراری بودن در Application Handler — نه در Domain
        var existing = await repository.Get(userName: command.UserName, token);
        if (existing is not null)
            throw new ExistingUserException();

        var id = repository.GetNextId();
        var user = UserAgg.Create(new CreateUserArg(id, command.MobileNumber ?? string.Empty));
        user.UserName = command.UserName;
        user.ChangePassword(PasswordHasher.HashPassword(command.Password, jwtOptions.Secret));

        // DisplayName برای JWT لازم است — اگر نام/نام‌خانوادگی ندادند، از UserName استفاده می‌شود
        var firstName = string.IsNullOrWhiteSpace(command.FirstName) ? command.UserName : command.FirstName;
        var lastName  = string.IsNullOrWhiteSpace(command.LastName)  ? command.UserName : command.LastName;
        user.Modify(new ModifyUserArg(firstName, lastName, command.Email));

        await repository.Add(user, token);
        await repository.Update(token);
        return user.Id;
    }
}
