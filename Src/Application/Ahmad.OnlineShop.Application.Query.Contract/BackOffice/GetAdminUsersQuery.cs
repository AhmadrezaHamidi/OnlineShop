using AhmadBase.Application.Query;
using BackOffice.Application.Dtos;
using Ahmad.OnlineShop.Domain.BackOffice.Enums;

namespace BackOffice.Application.Query.Queries;

public record GetAdminUsersQuery(
    int          Page     = 1,
    int          PageSize = 20,
    AdminStatus? Status   = null,
    AdminRole?   Role     = null
) : IQuery<PagedResult<AdminUserDto>>;
