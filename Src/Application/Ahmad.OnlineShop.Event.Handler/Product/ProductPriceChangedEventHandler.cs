using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Product;

/// <summary>
/// وقتی قیمت محصول تغییر می‌کند
/// → می‌توان به کاربرانی که این محصول را در wishlist دارند اطلاع داد
/// </summary>
public sealed class ProductPriceChangedEventHandler(ILogger<ProductPriceChangedEventHandler> logger)
    : IEventHandlerAsync<ProductPriceChangedEvent>
{
    public Task HandleAsync(ProductPriceChangedEvent @event, CancellationToken token)
    {
        var direction = @event.NewPrice < @event.OldPrice ? "کاهش" : "افزایش";

        logger.LogInformation(
            "قیمت محصول {ProductId} {Direction} یافت: {Old} → {New}",
            @event.ProductId, direction, @event.OldPrice, @event.NewPrice);

        // TODO: اطلاع‌رسانی به کاربران علاقمند
        return Task.CompletedTask;
    }
}
