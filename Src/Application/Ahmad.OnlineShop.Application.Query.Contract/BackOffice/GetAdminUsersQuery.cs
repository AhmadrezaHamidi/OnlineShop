using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Application.Query;

namespace BackOffice.Application.Query.Queries;

public record GetAdminUsersQuery(
    int          Page     = 1,
    int          PageSize = 20,
    AdminStatus? Status   = null,
    AdminRole?   Role     = null
) : IQuery<BackOfficePagedResult<GetAdminUserQueryResponse>>;

public record BackOfficePagedResult<T>(
    List<T> Items,
    int     TotalCount,
    int     Page,
    int     PageSize);
