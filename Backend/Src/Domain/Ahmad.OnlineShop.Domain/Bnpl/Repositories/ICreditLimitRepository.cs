using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;

namespace Ahmad.OnlineShop.Domain.Repositories;

public interface ICreditLimitRepository
{
    Task<CreditLimit?> GetByUserIdAsync(long userId, CancellationToken token = default);
    Task AddAsync(CreditLimit creditLimit, CancellationToken token = default);
    Task UpdateAsync(CreditLimit creditLimit, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
