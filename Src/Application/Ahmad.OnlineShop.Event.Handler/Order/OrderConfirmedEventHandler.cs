using Ahmad.OnlineShop.Application.Contract.Invoice.Commands;

namespace Ahmad.OnlineShop.Application.EventHandlers.Order;

/// <summary>
/// وقتی سفارش تأیید می‌شود (پرداخت موفق):
/// ۱. موجودی رزرو‌شده به صورت قطعی کسر می‌شود
/// ۲. فاکتور (رسید) برای سفارش صادر می‌شود (Observer Pattern)
/// </summary>
public sealed class OrderConfirmedEventHandler(
    IOrderRepository   orderRepo,
    IProductRepository productRepo,
    ICommandBus        commandBus,
    ILogger<OrderConfirmedEventHandler>? logger = null)
    : IEventHandlerAsync<OrderConfirmedEvent>
{
    public async Task HandleAsync(OrderConfirmedEvent @event, CancellationToken token)
    {
        var order = await orderRepo.GetByIdAsync(@event.OrderId, token);
        if (order is null) return;

        // ۱. کسر قطعی موجودی
        foreach (var item in order.Items)
        {
            var product = await productRepo.Get(item.ProductId, token);
            if (product is null) continue;

            product.ConfirmStock(item.Quantity);
            await productRepo.UpdateAsync(product, token);
        }

        // ۲. صدور فاکتور — Observer Pattern: event handler → command
        try
        {
            var invoiceId = await commandBus.Dispatch<long>(
                new CreateInvoiceCommand(@event.OrderId), token);
            logger?.LogInformation("فاکتور {InvoiceId} برای سفارش {OrderId} صادر شد",
                invoiceId, @event.OrderId);
        }
        catch (Exception ex)
        {
            // شکست در صدور فاکتور نباید تأیید سفارش را برگرداند
            logger?.LogError(ex, "خطا در صدور فاکتور برای سفارش {OrderId}", @event.OrderId);
        }
    }
}
