using Identity.Application.Dtos;

namespace Identity.Application.Query.Contracts;

/// <summary>
/// Read-side (query) repository for roles.
/// Implemented in the Infrastructure/Read layer (e.g. Dapper).
/// </summary>
public interface IRoleReadRepository
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken token = default);
}
