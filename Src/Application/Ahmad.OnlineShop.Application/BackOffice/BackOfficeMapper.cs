using Ahmad.OnlineShop.Domain.BackOffice.Args;
using BackOffice.Application.Commands;

namespace Ahmad.OnlineShop.Application.BackOffice.Mapper;

public static class BackOfficeMapper
{
    public static CreateAdminUserArg Map(this CreateAdminUserCommand command, long id)
        => new(id, command.FullName, command.Email, command.Role);
}
