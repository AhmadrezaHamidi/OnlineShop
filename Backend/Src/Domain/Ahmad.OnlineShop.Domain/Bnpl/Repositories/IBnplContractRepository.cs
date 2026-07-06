
using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;

namespace Ahmad.OnlineShop.Domain.Repositories;

public interface IBnplContractRepository
{
    Task<BnplContract?> GetByIdAsync(long id, CancellationToken token = default);
    Task<(List<BnplContract> Items, int Total)> GetByUserIdAsync(long userId, int page, int pageSize, CancellationToken token = default);
    Task AddAsync(BnplContract contract, CancellationToken token = default);
    Task UpdateAsync(BnplContract contract, CancellationToken token = default);
    Task<long> GetNextIdAsync();
    Task<long> GetNextInstallmentIdAsync();
}
