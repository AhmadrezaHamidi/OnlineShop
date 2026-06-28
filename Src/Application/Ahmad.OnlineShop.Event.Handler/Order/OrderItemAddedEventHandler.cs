namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی آیتم به سفارش افزوده می‌شود
/// → موجودی محصول در این مرحله Reserve می‌شود
/// </summary>
public sealed class OrderItemAddedEventHandler(IProductRepository productRepo)
    : IEventHandlerAsync<OrderItemAddedEvent>
{
    public async Task HandleAsync(OrderItemAddedEvent @event, CancellationToken token)
    {
        var product = await productRepo.Get(@event.ProductId, token);
        if (product is null) return;

        product.ReserveStock(@event.Quantity);
        await productRepo.UpdateAsync(product, token);
    }
}
