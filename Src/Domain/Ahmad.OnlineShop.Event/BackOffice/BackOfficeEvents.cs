using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.BackOffice.Events;

/// <summary>ادمین جدید ایجاد شد</summary>
public sealed record AdminUserCreatedEvent(
    long   AdminUserId,
    string FullName,
    string Email,
    int    Role          // AdminRole as int
) : IEvent;

/// <summary>وضعیت ادمین تغییر کرد</summary>
public sealed record AdminStatusChangedEvent(
    long AdminUserId,
    int  NewStatus       // AdminStatus as int
) : IEvent;

/// <summary>گزارش آماده شد</summary>
public sealed record ReportGeneratedEvent(
    long   ReportId,
    long   AdminUserId,
    int    Type,         // ReportType as int
    string FilePath
) : IEvent;

/// <summary>تولید گزارش شکست خورد</summary>
public sealed record ReportFailedEvent(
    long   ReportId,
    string Reason
) : IEvent;
