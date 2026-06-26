using AhmadBase.Application.Query;
using Identity.Application.Dtos;
using Identity.Application.Query.Contracts;
using Identity.Application.Query.Queries;

namespace Identity.Application.Query.Handlers;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserReadRepository _readRepo;

    public GetUsersQueryHandler(IUserReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<PagedResult<UserDto>> HandleAsync(GetUsersQuery query, CancellationToken token)
    {
        var (items, total) = await _readRepo.GetListAsync(
            page:     query.Page,
            pageSize: query.PageSize,
            search:   query.Search,
            status:   query.Status,
            token:    token);

        return new PagedResult<UserDto>(
            Items:      items,
            TotalCount: total,
            Page:       query.Page,
            PageSize:   query.PageSize);
    }
}
