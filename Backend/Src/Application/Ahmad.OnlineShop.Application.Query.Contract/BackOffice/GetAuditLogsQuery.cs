using AhmadBase.Application.Query;

namespace BackOffice.Application.Query.Queries;

public record GetAuditLogsQuery(
    long AdminId,
    int  Page     = 1,
    int  PageSize = 20
) : IQuery<List<GetAuditLogQueryResponse>>;

public sealed record GetAuditLogQueryResponse(
    long     Id,
    long?    AdminUserId,
    string   Action,
    string   EntityType,
    string   EntityId,
    string?  OldValue,
    string?  NewValue,
    DateTime CreatedAt);
