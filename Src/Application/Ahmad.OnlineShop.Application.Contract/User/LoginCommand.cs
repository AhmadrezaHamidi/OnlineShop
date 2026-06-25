using AhmadBase.Application;
using AhmadBase.Web.Securities;


namespace Ahmad.OnlineShop.Application.Contract.User;

public sealed record LoginCommand(
    string UserName,
    string Password
) : ICommand<TokenResponse>;
