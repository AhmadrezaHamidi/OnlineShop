using AhmadBase.Application;


namespace Ahmad.OnlineShop.Application.Contract.User;

public sealed record UpdateProfileCommand(
    long UserId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? MobileNumber
) : ICommand<object>;
