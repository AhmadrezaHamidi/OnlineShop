using AhmadBase.Application.Query;
using BackOffice.Application.Dtos;

namespace BackOffice.Application.Query.Queries;

public record GetAuditLogsQuery(
    long AdminId,
    int  Page     = 1,
    int  PageSize = 20
) : IQuery<List<AuditLogDto>>;
