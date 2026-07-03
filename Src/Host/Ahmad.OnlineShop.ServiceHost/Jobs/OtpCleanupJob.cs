using Ahmad.OnlineShop.Persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ahmad.OnlineShop.ServiceHost.Jobs;

/// <summary>
/// هر ساعت رکوردهای OTP منقضی‌شده را از دیتابیس حذف می‌کند
/// </summary>
[DisallowConcurrentExecution]
public sealed class OtpCleanupJob(
    IdentityAppDbContext db,
    ILogger<OtpCleanupJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.UtcNow;

        var deleted = await db.OtpRequests
            .Where(o => o.ExpiresAt < now)
            .ExecuteDeleteAsync(context.CancellationToken);

        if (deleted > 0)
            logger.LogInformation("OTP Cleanup: {Count} رکورد OTP منقضی‌شده حذف شد.", deleted);
    }
}
