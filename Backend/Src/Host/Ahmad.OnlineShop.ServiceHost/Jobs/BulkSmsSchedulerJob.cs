using AhmadBase.External.Service;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ahmad.OnlineShop.ServiceHost.Jobs;

/// <summary>
/// پیامک‌های بازاریابی زمان‌بندی‌شده‌ای که در صف باشند را ارسال می‌کند
/// پیام‌ها از جدول MarketingSmsQueue خوانده می‌شوند؛ پیامک‌های هم‌گروه دسته‌بندی می‌شوند
/// </summary>
[DisallowConcurrentExecution]
public sealed class BulkSmsSchedulerJob(
    ISmsService smsService,
    IConfiguration configuration,
    ILogger<BulkSmsSchedulerJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var connStr = configuration.GetConnectionString("CommandConnection") ?? string.Empty;

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(context.CancellationToken);

        // خواندن پیامک‌هایی که scheduled_at سررسیدشان رسیده و هنوز ارسال نشده‌اند
        var pending = new List<(long Id, string Phone, string Message)>();

        var selectCmd = new SqlCommand(
            """
            SELECT TOP 100 Id, PhoneNumber, Message
            FROM   MarketingSmsQueue
            WHERE  IsSent = 0
              AND  ScheduledAt <= GETUTCDATE()
            ORDER  BY ScheduledAt
            """, conn);

        await using (var reader = await selectCmd.ExecuteReaderAsync(context.CancellationToken))
        {
            while (await reader.ReadAsync(context.CancellationToken))
                pending.Add((reader.GetInt64(0), reader.GetString(1), reader.GetString(2)));
        }

        if (pending.Count == 0)
        {
            logger.LogInformation("BulkSms: هیچ پیامکی در صف نیست.");
            return;
        }

        // گروه‌بندی بر اساس متن پیام برای استفاده از SendBulkAsync
        var groups = pending.GroupBy(x => x.Message);
        int sent   = 0;

        foreach (var group in groups)
        {
            var phones = group.Select(g => g.Phone).ToList();
            try
            {
                await smsService.SendBulkAsync(phones, group.Key, context.CancellationToken);

                foreach (var (id, _, _) in group)
                {
                    var updateCmd = new SqlCommand(
                        "UPDATE MarketingSmsQueue SET IsSent = 1, SentAt = GETUTCDATE() WHERE Id = @Id",
                        conn);
                    updateCmd.Parameters.AddWithValue("@Id", id);
                    await updateCmd.ExecuteNonQueryAsync(context.CancellationToken);
                    sent++;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "BulkSms: خطا در ارسال پیامک دسته‌ای به {Count} شماره.", phones.Count);
            }
        }

        logger.LogInformation("BulkSms: {Sent}/{Total} پیامک ارسال شد.", sent, pending.Count);
    }
}
