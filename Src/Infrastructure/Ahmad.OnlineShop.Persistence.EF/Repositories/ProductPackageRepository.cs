using AhmadBase.Persistence.NHiLoHelper;
using Ahmad.OnlineShop.Domain.Discount.Aggregates;
using Ahmad.OnlineShop.Domain.Discount.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public sealed class ProductPackageRepository(
    ApplicationDbContext context,
    IHiLoIdGenerator     hiLo) : IProductPackageRepository
{
    public Task<ProductPackage?> GetByIdAsync(long id, CancellationToken token = default)
        => context.ProductPackages
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<(List<ProductPackage> Items, int Total)> GetListAsync(
        int page, int pageSize, bool? isActive, CancellationToken token = default)
    {
        var q = context.ProductPackages.Include(x => x.Items).AsQueryable();
        if (isActive.HasValue) q = q.Where(x => x.IsActive == isActive.Value);

        var total = await q.CountAsync(token);
        var items = await q.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token);
        return (items, total);
    }

    public async Task AddAsync(ProductPackage package, CancellationToken token = default)
        => await context.ProductPackages.AddAsync(package, token);

    public Task UpdateAsync(ProductPackage package, CancellationToken token = default)
    {
        context.ProductPackages.Update(package);
        return Task.CompletedTask;
    }

    public long GetNextId()     => hiLo.GetNextId<ProductPackage>();
    public long GetNextItemId() => hiLo.GetNextId<ProductPackage>() * 100 + 1;
}
