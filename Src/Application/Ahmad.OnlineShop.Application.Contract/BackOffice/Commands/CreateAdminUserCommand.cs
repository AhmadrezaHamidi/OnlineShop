using AhmadBase.Application;
using BackOffice.Domain.Enums;

namespace BackOffice.Application.Commands;

public record CreateAdminUserCommand(
    string    FullName,
    string    Email,
    AdminRole Role
) : ICommand<long>;
