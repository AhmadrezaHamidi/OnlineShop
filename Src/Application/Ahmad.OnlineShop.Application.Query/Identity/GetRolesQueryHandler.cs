using AhmadBase.Application.Query;
using Identity.Application.Dtos;
using Identity.Application.Query.Contracts;
using Identity.Application.Query.Queries;

namespace Identity.Application.Query.Handlers;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, IReadOnlyList<RoleDto>>
{
    private readonly IRoleReadRepository _readRepo;

    public GetRolesQueryHandler(IRoleReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public async Task<IReadOnlyList<RoleDto>> HandleAsync(GetRolesQuery query, CancellationToken token)
    {
        return await _readRepo.GetAllAsync(token);
    }
}
