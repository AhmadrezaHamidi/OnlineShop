using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Repositories;
using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;
using OrderAggregate = Ahmad.OnlineShop.Domain.Order.Aggregates.Order;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public sealed class OrderRepository(
    ApplicationDbContext context,
    IHiLoIdGenerator      hiLoGenerator) : IOrderRepository
{
    public async Task<OrderAggregate?> GetByIdAsync(long id, CancellationToken token = default)
        => await context.Orders
            .Include(x => x.Items)
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<(List<OrderAggregate> Items, int Total)> GetListAsync(
        int page, int pageSize, long? userId, OrderStatus? status, CancellationToken token = default)
    {
        var query = context.Orders
            .Include(x => x.Items)
            .Include(x => x.Payments)
            .AsQueryable();

        if (userId.HasValue) query = query.Where(x => x.UserId == userId.Value);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);

        var total = await query.CountAsync(token);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return (items, total);
    }

    public async Task AddAsync(OrderAggregate order, CancellationToken token = default)
        => await context.Orders.AddAsync(order, token);

    public Task UpdateAsync(OrderAggregate order, CancellationToken token = default)
    {
        context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<OrderAggregate>());
}
