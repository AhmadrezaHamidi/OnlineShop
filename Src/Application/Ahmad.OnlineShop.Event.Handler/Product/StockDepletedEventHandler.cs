using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Product;

/// <summary>
/// وقتی موجودی محصول به صفر یا زیر آستانه رسید
/// → هشدار به فروشنده ارسال می‌شود
/// </summary>
public sealed class StockDepletedEventHandler(
    IProductRepository              productRepo,
    ILogger<StockDepletedEventHandler> logger)
    : IEventHandlerAsync<StockDepletedEvent>
{
    public async Task HandleAsync(StockDepletedEvent @event, CancellationToken token)
    {
        var product = await productRepo.Get(@event.ProductId, token);
        if (product is null) return;

        if (@event.CurrentQuantity <= 0)
        {
            logger.LogWarning(
                "موجودی محصول '{Name}' (Id:{Id}) به صفر رسید — فروشنده {SellerId} باید موجودی تأمین کند",
                product.Name, @event.ProductId, product.SellerId);
        }
        else
        {
            logger.LogWarning(
                "موجودی محصول '{Name}' (Id:{Id}) کم شد — {Qty} عدد باقی مانده",
                product.Name, @event.ProductId, @event.CurrentQuantity);
        }

        // TODO: ارسال notification به فروشنده
    }
}
