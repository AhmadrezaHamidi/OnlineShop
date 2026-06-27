/// <summary>پیاده‌سازی Fake برای IOrderRepository</summary>
namespace Ahmad.OnlineShop.Application.Tests.Fakes;

public class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<long, OrderAgg> _store = new();
    private long _nextId = 1;

    public OrderAgg? Added   { get; private set; }
    public OrderAgg? Updated { get; private set; }

    public Task<OrderAgg?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<(List<OrderAgg> Items, int Total)> GetListAsync(
        int page, int pageSize, long? userId, OrderStatus? status, CancellationToken token = default)
    {
        var items = _store.Values.ToList();
        return Task.FromResult((items, items.Count));
    }

    public Task AddAsync(OrderAgg order, CancellationToken token = default)
    {
        Added = order;
        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(OrderAgg order, CancellationToken token = default)
    {
        Updated = order;
        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync() => Task.FromResult(_nextId++);

    public void Seed(OrderAgg order) => _store[order.Id] = order;
}
