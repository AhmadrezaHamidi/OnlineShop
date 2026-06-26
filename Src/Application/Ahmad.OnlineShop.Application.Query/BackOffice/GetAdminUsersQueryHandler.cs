using AhmadBase.Application.Query;
using BackOffice.Application.Dtos;
using BackOffice.Application.Query.Contracts;
using BackOffice.Application.Query.Queries;

namespace BackOffice.Application.Query.Handlers;

public class GetAdminUsersQueryHandler : IQueryHandler<GetAdminUsersQuery, PagedResult<AdminUserDto>>
{
    private readonly IAdminUserReadRepository _readRepo;

    public GetAdminUsersQueryHandler(IAdminUserReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<PagedResult<AdminUserDto>> HandleAsync(GetAdminUsersQuery query, CancellationToken token)
    {
        var (items, total) = await _readRepo.GetListAsync(
            page:     query.Page,
            pageSize: query.PageSize,
            status:   query.Status,
            role:     query.Role,
            token:    token);

        return new PagedResult<AdminUserDto>(
            Items:      items,
            TotalCount: total,
            Page:       query.Page,
            PageSize:   query.PageSize);
    }
}
