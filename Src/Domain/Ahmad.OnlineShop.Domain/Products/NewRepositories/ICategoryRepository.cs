using Ahmad.OnlineShop.Domain.Entities;

namespace Ahmad.OnlineShop.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(long id, CancellationToken token = default);
    Task<List<Category>> GetAllAsync(CancellationToken token = default);
    Task AddAsync(Category category, CancellationToken token = default);
    Task UpdateAsync(Category category, CancellationToken token = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
