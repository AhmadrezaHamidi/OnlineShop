using Ahmad.OnlineShop.Domain.Discount.Aggregates;

namespace Ahmad.OnlineShop.Domain.Discount.Repositories;

public interface IProductPackageRepository
{
    Task<ProductPackage?> GetByIdAsync(long id, CancellationToken token = default);
    Task<(List<ProductPackage> Items, int Total)> GetListAsync(
        int page, int pageSize, bool? isActive, CancellationToken token = default);
    Task AddAsync(ProductPackage package, CancellationToken token = default);
    Task UpdateAsync(ProductPackage package, CancellationToken token = default);
    long GetNextId();
    long GetNextItemId();
}
