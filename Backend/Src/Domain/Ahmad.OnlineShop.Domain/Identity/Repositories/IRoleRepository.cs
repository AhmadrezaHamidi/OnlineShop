using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(long id, CancellationToken token = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken token = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken token = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken token = default);
    Task AddAsync(Role role, CancellationToken token = default);
    Task UpdateAsync(Role role, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
