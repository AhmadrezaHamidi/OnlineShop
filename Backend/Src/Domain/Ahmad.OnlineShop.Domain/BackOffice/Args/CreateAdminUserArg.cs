using Ahmad.OnlineShop.Domain.BackOffice.Enums;

namespace Ahmad.OnlineShop.Domain.BackOffice.Args;

public sealed record CreateAdminUserArg(
long Id,
string FullName,
string Email,
AdminRole Role
);
public sealed record CreateAuditLogArg(
    long Id,
    long? AdminUserId,
    string Action,
    string EntityType,
    string EntityId,
    string? OldValue = null,
    string? NewValue = null
);

public sealed record CreateReportArg(
    long Id,
    long? AdminUserId,
    ReportType Type
);