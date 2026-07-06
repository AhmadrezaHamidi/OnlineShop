using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی سفارش ارسال می‌شود
/// → می‌توان پیامک/ایمیل ارسال اطلاع‌رسانی ارسال کرد
/// </summary>
public sealed class OrderShippedEventHandler(ILogger<OrderShippedEventHandler> logger)
    : IEventHandlerAsync<OrderShippedEvent>
{
    public Task HandleAsync(OrderShippedEvent @event, CancellationToken token)
    {
        logger.LogInformation(
            "سفارش {OrderId} برای کاربر {UserId} ارسال شد",
            @event.OrderId, @event.UserId);

        // TODO: ارسال SMS/Push اطلاع‌رسانی به مشتری
        return Task.CompletedTask;
    }
}
