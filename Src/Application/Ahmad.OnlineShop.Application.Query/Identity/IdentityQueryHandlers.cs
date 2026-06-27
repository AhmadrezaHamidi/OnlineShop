using Identity.Application.Query.Contracts;
using Identity.Application.Query.Queries;
using Identity.Domain.Exceptions;

namespace Identity.Application.Query.Handlers;

public sealed class IdentityQueryHandlers(
    IUserReadRepository userReadRepo,
    IRoleReadRepository roleReadRepo) :
    IQueryHandler<GetUserQuery,  GetUserQueryResponse>,
    IQueryHandler<GetUsersQuery, IdentityPagedResult<GetUserQueryResponse>>,
    IQueryHandler<GetRolesQuery, IReadOnlyList<GetRoleQueryResponse>>
{
    public async Task<GetUserQueryResponse> HandleAsync(GetUserQuery query, CancellationToken token)
    {
        var user = await userReadRepo.GetByIdAsync(query.UserId, token)
            ?? throw new UserNotFoundException();

        return user;
    }

    public async Task<IdentityPagedResult<GetUserQueryResponse>> HandleAsync(GetUsersQuery query, CancellationToken token)
    {
        var (items, total) = await userReadRepo.GetListAsync(
            page:     query.Page,
            pageSize: query.PageSize,
            search:   query.Search,
            status:   query.Status,
            token:    token);

        return new IdentityPagedResult<GetUserQueryResponse>(
            Items:      items,
            TotalCount: total,
            Page:       query.Page,
            PageSize:   query.PageSize);
    }

    public async Task<IReadOnlyList<GetRoleQueryResponse>> HandleAsync(GetRolesQuery query, CancellationToken token)
        => await roleReadRepo.GetAllAsync(token);
}
