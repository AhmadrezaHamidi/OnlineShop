using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record CreateAdminUserCommand(
    string    FullName,
    string    Email,
    AdminRole Role
) : ICommand<long>;
