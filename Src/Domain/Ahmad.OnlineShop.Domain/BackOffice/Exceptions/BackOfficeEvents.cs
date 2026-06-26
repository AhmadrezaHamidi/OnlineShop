using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.BackOffice.Events;

/// <summary>ادمین جدید ایجاد شد</summary>
public sealed record AdminUserCreatedEvent(
    long      AdminUserId,
    string    FullName,
    string    Email,
    AdminRole Role
) : IEvent;

/// <summary>وضعیت ادمین تغییر کرد</summary>
public sealed record AdminStatusChangedEvent(
    long        AdminUserId,
    AdminStatus NewStatus
) : IEvent;

/// <summary>گزارش آماده شد</summary>
public sealed record ReportGeneratedEvent(
    long       ReportId,
    long       AdminUserId,
    ReportType Type,
    string     FilePath
) : IEvent;

/// <summary>تولید گزارش شکست خورد</summary>
public sealed record ReportFailedEvent(
    long   ReportId,
    string Reason
) : IEvent;
