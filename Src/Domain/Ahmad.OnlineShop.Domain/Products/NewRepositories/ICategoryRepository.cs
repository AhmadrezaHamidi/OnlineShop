using Ahmad.OnlineShop.Domain.Products;

namespace Ahmad.OnlineShop.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> Get(long id, CancellationToken token = default);
    Task<List<Category>> Gets(CancellationToken token = default);
    Task Add(Category category, CancellationToken token = default);
    Task Update(Category category, CancellationToken token = default);
    Task<bool> ExistsByName(string name, CancellationToken token = default);
    long GetNextId();
}
