using AhmadBase.Application;
using AhmadBase.Web.Securities;


namespace Ahmad.OnlineShop.Application.Contract.User;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : ICommand<TokenResponse>;
