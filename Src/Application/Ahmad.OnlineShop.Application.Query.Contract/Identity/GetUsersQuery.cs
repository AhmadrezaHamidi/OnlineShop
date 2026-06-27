using AhmadBase.Application.Query;
using Identity.Domain.Enums;

namespace Identity.Application.Query.Queries;

public record GetUsersQuery(
    int         Page     = 1,
    int         PageSize = 20,
    string?     Search   = null,
    UserStatus? Status   = null
) : IQuery<IdentityPagedResult<GetUserQueryResponse>>;

public record IdentityPagedResult<T>(
    IReadOnlyList<T> Items,
    int              TotalCount,
    int              Page,
    int              PageSize)
{
    public int  TotalPages      => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasNextPage     => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
