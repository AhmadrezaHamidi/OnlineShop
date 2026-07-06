namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی آیتم از سفارش حذف می‌شود
/// → موجودی رزرو‌شده آزاد می‌شود
/// </summary>
public sealed class OrderItemRemovedEventHandler(IProductRepository productRepo)
    : IEventHandlerAsync<OrderItemRemovedEvent>
{
    public async Task HandleAsync(OrderItemRemovedEvent @event, CancellationToken token)
    {
        var product = await productRepo.Get(@event.ProductId, token);
        if (product is null) return;

        product.ReleaseStock(@event.Quantity);
        await productRepo.UpdateAsync(product, token);
    }
}
