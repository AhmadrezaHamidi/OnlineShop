using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Product;

/// <summary>
/// وقتی موجودی محصول افزایش می‌یابد
/// → اگه قبلاً out-of-stock بوده، می‌توان به کاربران علاقمند اطلاع داد
/// </summary>
public sealed class StockReplenishedEventHandler(ILogger<StockReplenishedEventHandler> logger)
    : IEventHandlerAsync<StockReplenishedEvent>
{
    public Task HandleAsync(StockReplenishedEvent @event, CancellationToken token)
    {
        logger.LogInformation(
            "موجودی محصول {ProductId} تأمین شد: +{Added} عدد (کل: {Total})",
            @event.ProductId, @event.AddedQuantity, @event.NewTotalQuantity);

        // TODO: اطلاع‌رسانی به کاربرانی که منتظر موجودی بودند
        return Task.CompletedTask;
    }
}
