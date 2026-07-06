using Identity.Application.Query.Queries;

namespace Identity.Application.Query.Contracts;

public interface IRoleReadRepository
{
    Task<IReadOnlyList<GetRoleQueryResponse>> GetAllAsync(CancellationToken token = default);
}
