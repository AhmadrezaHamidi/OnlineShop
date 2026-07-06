using AhmadBase.Application.Query;
using Identity.Domain.Enums;

namespace Identity.Application.Query.Queries;

public record GetUsersQuery(
    int         Page     = 1,
    int         PageSize = 20,
    string?     Search   = null,
    UserStatus? Status   = null
) : IQuery<PagedResult<GetUserQueryResponse>>;
