using AhmadBase.Application;


namespace Ahmad.OnlineShop.Application.Contract.User;

public sealed record ChangePasswordCommand(
    long UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : ICommand<long>;
