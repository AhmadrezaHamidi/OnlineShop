using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record ChangeAdminRoleCommand(
    long      AdminId,
    AdminRole NewRole
) : ICommand<long>;
