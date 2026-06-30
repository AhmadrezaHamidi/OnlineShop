using AhmadBase.Persistence.NHiLoHelper;
using Ahmad.OnlineShop.Domain.Discount.Repositories;
using Microsoft.EntityFrameworkCore;
using DiscountAggregate = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public sealed class DiscountRepository(
    ApplicationDbContext context,
    IHiLoIdGenerator     hiLo) : IDiscountRepository
{
    public Task<DiscountAggregate?> GetByIdAsync(long id, CancellationToken token = default)
        => context.Discounts.FirstOrDefaultAsync(x => x.Id == id, token);

    public Task<DiscountAggregate?> GetByCodeAsync(string code, CancellationToken token = default)
        => context.Discounts.FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant(), token);

    public Task<bool> CodeExistsAsync(string code, CancellationToken token = default)
        => context.Discounts.AnyAsync(x => x.Code == code.ToUpperInvariant(), token);

    public async Task<(List<DiscountAggregate> Items, int Total)> GetListAsync(
        int page, int pageSize, bool? isActive, CancellationToken token = default)
    {
        var q = context.Discounts.AsQueryable();
        if (isActive.HasValue) q = q.Where(x => x.IsActive == isActive.Value);

        var total = await q.CountAsync(token);
        var items = await q.OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(token);
        return (items, total);
    }

    public async Task AddAsync(DiscountAggregate discount, CancellationToken token = default)
        => await context.Discounts.AddAsync(discount, token);

    public Task UpdateAsync(DiscountAggregate discount, CancellationToken token = default)
    {
        context.Discounts.Update(discount);
        return Task.CompletedTask;
    }

    public long GetNextId()
        => hiLo.GetNextId<DiscountAggregate>();
}
