using Ahmad.OnlineShop.Domain.BackOffice.Args;
using Ahmad.OnlineShop.Domain.BackOffice.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.BackOffice.Entities;

public sealed class AuditLog : TEntity<long>
{
    public long?    AdminUserId { get; private set; }
    public string   Action      { get; private set; } = string.Empty;
    public string   EntityType  { get; private set; } = string.Empty;
    public string   EntityId    { get; private set; } = string.Empty;
    public string?  OldValue    { get; private set; }
    public string?  NewValue    { get; private set; }
    public DateTime CreatedAt   { get; private set; }

    private AuditLog() { }

    internal AuditLog(CreateAuditLogArg arg)
    {
        GuardAction(arg.Action);
        GuardEntityType(arg.EntityType);

        Id          = arg.Id;
        AdminUserId = arg.AdminUserId;
        Action      = arg.Action;
        EntityType  = arg.EntityType;
        EntityId    = arg.EntityId;
        OldValue    = arg.OldValue;
        NewValue    = arg.NewValue;
        CreatedAt   = DateTime.UtcNow;
    }

    private static void GuardAction(string action)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new AuditInvalidActionException();
    }

    private static void GuardEntityType(string entityType)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new AuditInvalidEntityException();
    }
}
