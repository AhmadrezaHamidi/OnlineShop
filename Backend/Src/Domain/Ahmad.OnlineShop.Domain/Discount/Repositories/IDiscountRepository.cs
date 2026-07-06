using DiscountAggregate = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Domain.Discount.Repositories;

public interface IDiscountRepository
{
    Task<DiscountAggregate?> GetByIdAsync(long id, CancellationToken token = default);
    Task<DiscountAggregate?> GetByCodeAsync(string code, CancellationToken token = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken token = default);
    Task<(List<DiscountAggregate> Items, int Total)> GetListAsync(
        int page, int pageSize, bool? isActive, CancellationToken token = default);
    Task AddAsync(DiscountAggregate discount, CancellationToken token = default);
    Task UpdateAsync(DiscountAggregate discount, CancellationToken token = default);
    long GetNextId();
}
