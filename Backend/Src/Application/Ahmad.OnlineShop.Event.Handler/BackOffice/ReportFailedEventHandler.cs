using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.BackOffice;

/// <summary>
/// وقتی تولید گزارش شکست می‌خورد
/// → هشدار فوری به SuperAdmin
/// </summary>
public sealed class ReportFailedEventHandler(ILogger<ReportFailedEventHandler> logger)
    : IEventHandlerAsync<ReportFailedEvent>
{
    public Task HandleAsync(ReportFailedEvent @event, CancellationToken token)
    {
        logger.LogError(
            "تولید گزارش {ReportId} شکست خورد — دلیل: {Reason}",
            @event.ReportId, @event.Reason);

        // TODO: ارسال alert به SuperAdmin
        return Task.CompletedTask;
    }
}
