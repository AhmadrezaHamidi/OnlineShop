using Ahmad.OnlineShop.Domain.BackOffice.Args;
using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using Ahmad.OnlineShop.Domain.BackOffice.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.BackOffice.Entities;


public sealed class Report : TEntity<long>
{
    public long? AdminUserId { get; private set; }
    public ReportType Type { get; private set; }
    public ReportStatus Status { get; private set; }
    public string? FilePath { get; private set; }
    public DateTime? GeneratedAt { get; private set; }
    public string? FailReason { get; private set; }

    private Report() { }

    internal Report(CreateReportArg arg)
    {
        Id = arg.Id;
        AdminUserId = arg.AdminUserId;
        Type = arg.Type;
        Status = ReportStatus.Pending;
    }

    internal void MarkGenerating()
    {
        Status = ReportStatus.Generating;
    }

    internal void MarkCompleted(string filePath)
    {
        GuardFilePathNotEmpty(filePath);
        GuardNotAlreadyCompleted();

        Status = ReportStatus.Completed;
        FilePath = filePath;
        GeneratedAt = DateTime.UtcNow;
    }

    internal void MarkFailed(string reason)
    {
        Status = ReportStatus.Failed;
        FailReason = reason;
    }


    private static void GuardFilePathNotEmpty(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ReportNotFoundException();
    }

    private void GuardNotAlreadyCompleted()
    {
        if (Status == ReportStatus.Completed)
            throw new ReportAlreadyCompletedException();
    }
}