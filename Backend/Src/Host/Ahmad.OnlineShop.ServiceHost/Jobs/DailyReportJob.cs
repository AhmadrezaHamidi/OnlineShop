using Ahmad.OnlineShop.Persistence.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ahmad.OnlineShop.ServiceHost.Jobs;

/// <summary>
/// هر شب آمار روزانه فروش، سفارش و کاربران جدید را لاگ می‌کند
/// </summary>
[DisallowConcurrentExecution]
public sealed class DailyReportJob(
    ApplicationDbContext appDb,
    IdentityAppDbContext identityDb,
    ILogger<DailyReportJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var today    = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var ct       = context.CancellationToken;

        var orderCount = await appDb.Orders
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .CountAsync(ct);

        var revenue = await appDb.Payments
            .Where(p => p.PaidAt >= today && p.PaidAt < tomorrow)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        var newUsers = await identityDb.IdentityUsers
            .Where(u => u.CreatedAt >= today && u.CreatedAt < tomorrow)
            .CountAsync(ct);

        logger.LogInformation(
            "Daily Report [{Date}] — سفارش: {Orders} | درآمد: {Revenue:N0} تومان | کاربر جدید: {Users}",
            today.ToString("yyyy-MM-dd"), orderCount, revenue, newUsers);
    }
}
