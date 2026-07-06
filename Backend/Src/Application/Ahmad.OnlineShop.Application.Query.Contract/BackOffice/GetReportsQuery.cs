using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Application.Query;

namespace BackOffice.Application.Query.Queries;

public record GetReportsQuery(
    long AdminId,
    int  Page     = 1,
    int  PageSize = 20
) : IQuery<List<GetReportQueryResponse>>;

public sealed record GetReportQueryResponse(
    long         Id,
    long?        AdminUserId,
    ReportType   Type,
    ReportStatus Status,
    string?      FilePath,
    DateTime?    GeneratedAt,
    string?      FailReason);
