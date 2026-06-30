using Ahmad.OnlineShop.Domain.Products;
using Ahmad.OnlineShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public sealed class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public Task<Category?> Get(long id, CancellationToken token = default)
        => context.Categories.FirstOrDefaultAsync(c => c.Id == id, token);

    public Task<List<Category>> Gets(CancellationToken token = default)
        => context.Categories.ToListAsync(token);

    public async Task Add(Category category, CancellationToken token = default)
        => await context.Categories.AddAsync(category, token);

    public Task Update(Category category, CancellationToken token = default)
    {
        context.Categories.Update(category);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByName(string name, CancellationToken token = default)
        => context.Categories.AnyAsync(c => c.Name == name, token);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken token = default)
        => ExistsByName(name, token);

    public async Task<long> GetNextIdAsync()
    {
        var max = await context.Categories.MaxAsync(c => (long?)c.Id) ?? 0;
        return max + 1;
    }

    public long GetNextId() => (context.Categories.Max(c => (long?)c.Id) ?? 0) + 1;

    public Task<Category?> GetByIdAsync(long id, CancellationToken token = default) => Get(id, token);
    public Task<List<Category>> GetAllAsync(CancellationToken token = default)       => Gets(token);
    public Task AddAsync(Category category, CancellationToken token = default)       => Add(category, token);
    public Task UpdateAsync(Category category, CancellationToken token = default)    => Update(category, token);
}
