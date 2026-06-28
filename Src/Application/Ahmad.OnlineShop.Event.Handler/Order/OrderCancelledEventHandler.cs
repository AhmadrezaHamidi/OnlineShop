namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی سفارش لغو می‌شود
/// → موجودی رزرو‌شده برای همه آیتم‌ها آزاد می‌شود
/// </summary>
public sealed class OrderCancelledEventHandler(IProductRepository productRepo)
    : IEventHandlerAsync<OrderCancelledEvent>
{
    public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken token)
    {
        foreach (var item in @event.Items)
        {
            var product = await productRepo.Get(item.ProductId, token);
            if (product is null) continue;

            product.ReleaseStock(item.Quantity);
            await productRepo.UpdateAsync(product, token);
        }
    }
}
