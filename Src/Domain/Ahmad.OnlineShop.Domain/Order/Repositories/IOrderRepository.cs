using OrderAgg = Ahmad.OnlineShop.Domain.Order.Aggregates;
namespace Ahmad.OnlineShop.Domain.Repositories;

public interface IOrderRepository
{
    Task<OrderAgg.Order?> GetByIdAsync(long id, CancellationToken token = default);
    Task<(List<OrderAgg.Order> Items, int Total)> GetListAsync(int page, int pageSize, long? userId, OrderStatus? status, CancellationToken token = default);
    Task AddAsync(OrderAgg.Order order, CancellationToken token = default);
    Task UpdateAsync(OrderAgg.Order order, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
