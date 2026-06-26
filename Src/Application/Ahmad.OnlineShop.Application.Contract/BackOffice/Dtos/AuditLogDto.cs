namespace BackOffice.Application.Dtos;

public sealed record AuditLogDto(
    long      Id,
    long?     AdminUserId,
    string    Action,
    string    EntityType,
    string    EntityId,
    string?   OldValue,
    string?   NewValue,
    DateTime  CreatedAt
);
