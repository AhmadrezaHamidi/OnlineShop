using Identity.Domain.Aggregates;

namespace Identity.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken token = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken token = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken token = default);
    Task AddAsync(User user, CancellationToken token = default);
    Task UpdateAsync(User user, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
