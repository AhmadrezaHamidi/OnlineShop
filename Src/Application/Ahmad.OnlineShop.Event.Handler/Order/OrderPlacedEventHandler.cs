using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی سفارش ثبت نهایی می‌شود
/// → لاگ می‌زند و می‌توان در آینده notification ارسال کرد
/// </summary>
public sealed class OrderPlacedEventHandler(ILogger<OrderPlacedEventHandler> logger)
    : IEventHandlerAsync<OrderPlacedEvent>
{
    public Task HandleAsync(OrderPlacedEvent @event, CancellationToken token)
    {
        logger.LogInformation(
            "سفارش {OrderId} توسط کاربر {UserId} ثبت شد — مبلغ: {Amount} | روش: {Method}",
            @event.OrderId, @event.UserId, @event.TotalAmount, @event.PaymentMethod);

        return Task.CompletedTask;
    }
}
