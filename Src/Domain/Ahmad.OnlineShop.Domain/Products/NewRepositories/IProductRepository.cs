using Ahmad.OnlineShop.Domain.Aggregates;
using Ahmad.OnlineShop.Domain.Enums;

namespace Ahmad.OnlineShop.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(long id, CancellationToken token = default);
    Task<(List<Product> Items, int Total)> GetListAsync(int page, int pageSize, string? search, long? categoryId, ProductStatus? status, CancellationToken token = default);
    Task AddAsync(Product product, CancellationToken token = default);
    Task UpdateAsync(Product product, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
