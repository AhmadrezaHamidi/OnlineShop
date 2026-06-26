using Ahmad.OnlineShop.Domain.BackOffice.Enums;

namespace BackOffice.Application.Dtos;

public sealed record ReportDto(
    long Id,
    long? AdminUserId,
    ReportType Type,
    ReportStatus Status,
    string? FilePath,
    DateTime? GeneratedAt,
    string? FailReason
);
