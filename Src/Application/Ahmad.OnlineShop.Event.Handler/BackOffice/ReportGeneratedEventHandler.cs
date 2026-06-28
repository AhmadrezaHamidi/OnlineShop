using Microsoft.Extensions.Logging;

namespace Ahmad.OnlineShop.Application.EventHandlers.BackOffice;

/// <summary>
/// وقتی گزارش آماده می‌شود
/// → اطلاع به مدیر درخواست‌دهنده
/// </summary>
public sealed class ReportGeneratedEventHandler(ILogger<ReportGeneratedEventHandler> logger)
    : IEventHandlerAsync<ReportGeneratedEvent>
{
    public Task HandleAsync(ReportGeneratedEvent @event, CancellationToken token)
    {
        logger.LogInformation(
            "گزارش {ReportId} نوع {Type} برای مدیر {AdminId} آماده شد — مسیر: {Path}",
            @event.ReportId, @event.Type, @event.AdminUserId, @event.FilePath);

        // TODO: ارسال ایمیل/notification به مدیر با لینک دانلود
        return Task.CompletedTask;
    }
}
