using Ahmad.OnlineShop.Domain.Products;
using Ahmad.OnlineShop.Domain.Products.Enums;

namespace Ahmad.OnlineShop.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> Get(long id, CancellationToken token = default);
    Task<(List<Product> Items, int Total)> GetListAsync(int page, int pageSize, string? search, long? categoryId, ProductStatus? status, CancellationToken token = default);
    Task AddAsync(Product product, CancellationToken token = default);
    Task UpdateAsync(Product product, CancellationToken token = default);
    long GetNextId();
}
