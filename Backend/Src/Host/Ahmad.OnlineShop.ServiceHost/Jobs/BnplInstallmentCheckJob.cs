using Ahmad.OnlineShop.Domain.Bnpl.Enums;
using Ahmad.OnlineShop.Persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ahmad.OnlineShop.ServiceHost.Jobs;

/// <summary>
/// هر شب اقساط سررسید-گذشته BNPL را پیدا کرده و وضعیتشان را Overdue می‌کند
/// از ExecuteUpdateAsync برای bulk update بدون بارگذاری entity ها در حافظه
/// </summary>
[DisallowConcurrentExecution]
public sealed class BnplInstallmentCheckJob(
    ApplicationDbContext db,
    ILogger<BnplInstallmentCheckJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.UtcNow;

        var updated = await db.Installments
            .Where(i => i.Status == InstallmentStatus.Pending && i.DueDate < now)
            .ExecuteUpdateAsync(
                s => s.SetProperty(i => i.Status, InstallmentStatus.Overdue),
                context.CancellationToken);

        if (updated == 0)
            logger.LogInformation("BNPL Check: هیچ قسط سررسید-گذشته‌ای یافت نشد.");
        else
            logger.LogWarning("BNPL Check: {Count} قسط سررسید-گذشته به حالت Overdue تغییر یافت.", updated);
    }
}
