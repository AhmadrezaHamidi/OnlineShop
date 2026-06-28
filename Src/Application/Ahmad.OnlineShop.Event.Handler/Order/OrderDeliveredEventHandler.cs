using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی سفارش تحویل داده می‌شود
/// → می‌توان درخواست نظرسنجی ارسال کرد
/// </summary>
public sealed class OrderDeliveredEventHandler(ILogger<OrderDeliveredEventHandler> logger)
    : IEventHandlerAsync<OrderDeliveredEvent>
{
    public Task HandleAsync(OrderDeliveredEvent @event, CancellationToken token)
    {
        logger.LogInformation(
            "سفارش {OrderId} به کاربر {UserId} تحویل داده شد",
            @event.OrderId, @event.UserId);

        // TODO: ارسال درخواست نظرسنجی و امتیاز
        return Task.CompletedTask;
    }
}
