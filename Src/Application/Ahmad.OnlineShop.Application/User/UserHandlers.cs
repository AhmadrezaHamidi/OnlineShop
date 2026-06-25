using Ahmad.OnlineShop.Application.Contract.User;
using Ahmad.OnlineShop.Domain.Users;
using Ahmad.OnlineShop.Domain.Users.Args;
using Ahmad.OnlineShop.Config;
using AhmadBase.Application;
using AhmadBase.Web.Securities;
using AhmadBase.Helper.Cryptography;
using Ahmad.OnlineShop.Application.Exceptions;
using Ahmad.userMa.ngemnt.Config;

namespace Ahmad.OnlineShop.Application.User;

public class UserHandlers(
    IUserRepository repository,
    JwtHandler jwtHandler,
    IUserService userService
    , JWTOptions jwtOptions) : ICommandHandler<LoginCommand, TokenResponse>,
      ICommandHandler<ChangePasswordCommand,long>,
      ICommandHandler<LogoutCommand, long>,
      ICommandHandler<RefreshTokenCommand, TokenResponse>,
      ICommandHandler<RegisterCommand, object>
{
    public async Task<TokenResponse> Handle(LoginCommand command, CancellationToken token)
    {
        var user = await repository.Get(userName: command.UserName, token);
        if (user is null)
            throw new UserNotFoundException();

        var hashedPassword = PasswordHasher.HashPassword(command.Password, jwtOptions.Secret);
        var sessionId = user.Login(hashedPassword);
        await repository.Update(token);

        var roles = user.Roles.Select(r => r.RoleId.ToString()).ToList();
        var tokenResponse = JwtHandler.GenerateTokens(new TokenRequest
        {
            UserId = user.Id.ToString(),
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            SessionId = sessionId,
            Roles = roles,
            ValidIssuer = jwtOptions.ValidIssuer,
            ValidAudience = jwtOptions.ValidAudience,
            TokenExpireMinutes = jwtOptions.TokenExpireMinutes,
            Secret = jwtOptions.Secret
        });

        return tokenResponse;
    }

    public async Task<long> Handle(ChangePasswordCommand command, CancellationToken token)
    {
        var user = await repository.Get(command.UserId, token);
        if (user is null) throw new UserNotFoundException();

        var currentHash = PasswordHasher.HashPassword(command.NewPassword, jwtOptions.Secret);
        user.Login(currentHash);

        if (command.NewPassword != command.ConfirmNewPassword)
            throw new PasswordMismatchException();

        user.ChangePassword(PasswordHasher.HashPassword(command.NewPassword, jwtOptions.Secret));
        await repository.Update(token);
        return user.Id;
    }

    public async Task<long> Handle(LogoutCommand command, CancellationToken token)
    {
        var user = await repository.Get(command.UserId, token);
        if (user is null) throw new UserNotFoundException();

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
        var user = await repository.Get(result.UserId, token);
        if (user is null) throw new UserNotFoundException();

        user.Logout(result.SessionId);
        var sessionId = user.Login(user.PasswordHash!);
        await repository.Update(token);

        var roles = user.Roles.Select(r => r.RoleId.ToString()).ToList();
        return JwtHandler.GenerateTokens(new TokenRequest
        {
            UserId = user.Id.ToString(),
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            SessionId = sessionId,
            Roles = roles,
            ValidIssuer = jwtOptions.ValidIssuer,
            ValidAudience = jwtOptions.ValidAudience,
            TokenExpireMinutes = jwtOptions.TokenExpireMinutes,
            Secret = jwtOptions.Secret
        });
    }

    public async Task<object> Handle(RegisterCommand command, CancellationToken token)
    {
        if (command.Password != command.ConfirmPassword)
            throw new PasswordMismatchException();

        var user = await repository.Get(userName: command.UserName, token);
        if (user is not null)
            throw new UserExistException();


        var id = repository.GetNextId();
         var use = await Domain.User.User.Create(new CreateUserArg(
             id, command.MobileNumber),userService, token);

        user.ChangePassword(PasswordHasher.HashPassword(command.Password, null));

        await repository.Add(user, token);

        return new { };
    }

    //public async Task<object> Handle(UpdateProfileCommand command, CancellationToken token)
    //{
    //    var user = await repository.Get(command.UserId, token);
    //    if (user is null) throw new InvalidOperationException("User not found");

    //    user.updae(command.DisplayName);
    //    await repository.Update(token);

    //    return new { };
    //}
}

