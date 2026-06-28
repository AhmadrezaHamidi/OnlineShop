namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی سفارش تأیید می‌شود (پرداخت موفق)
/// → موجودی رزرو‌شده به صورت قطعی کسر می‌شود (ConfirmStock)
/// </summary>
public sealed class OrderConfirmedEventHandler(IOrderRepository orderRepo, IProductRepository productRepo)
    : IEventHandlerAsync<OrderConfirmedEvent>
{
    public async Task HandleAsync(OrderConfirmedEvent @event, CancellationToken token)
    {
        var order = await orderRepo.GetByIdAsync(@event.OrderId, token);
        if (order is null) return;

        foreach (var item in order.Items)
        {
            var product = await productRepo.Get(item.ProductId, token);
            if (product is null) continue;

            // رزرو را به کسر قطعی تبدیل می‌کند
            product.ConfirmStock(item.Quantity);
            await productRepo.UpdateAsync(product, token);
        }
    }
}
