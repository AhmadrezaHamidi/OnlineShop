using Ahmad.OnlineShop.Domain.BackOffice.Args;
using Ahmad.OnlineShop.Domain.BackOffice.Entities;
using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using Ahmad.OnlineShop.Domain.BackOffice.Events;
using Ahmad.OnlineShop.Domain.BackOffice.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.BackOffice.Aggregates;

public sealed class AdminUser : AggregateRoot<long>
{
    private readonly List<AuditLog> _auditLogs = [];
    private readonly List<Report>   _reports   = [];

    public string      FullName  { get; private set; } = string.Empty;
    public string      Email     { get; private set; } = string.Empty;
    public AdminRole   Role      { get; private set; }
    public AdminStatus Status    { get; private set; }
    public DateTime    CreatedAt { get; private set; }

    public IReadOnlyCollection<AuditLog> AuditLogs => _auditLogs.AsReadOnly();
    public IReadOnlyCollection<Report>   Reports   => _reports.AsReadOnly();

    private AdminUser() { }

    private AdminUser(CreateAdminUserArg arg) : base(arg.Id)
    {
        FullName  = arg.FullName.Trim();
        Email     = arg.Email.Trim().ToLowerInvariant();
        Role      = arg.Role;
        Status    = AdminStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public static AdminUser Create(CreateAdminUserArg arg)
    {
        GuardFullName(arg.FullName);
        GuardEmail(arg.Email);

        var admin = new AdminUser(arg);
        admin.RaiseDomainEvent(new AdminUserCreatedEvent(arg.Id, arg.FullName, arg.Email, (int)arg.Role));
        return admin;
    }

    public void Activate()
    {
        GuardNotAlreadyActive();
        Status = AdminStatus.Active;
        RaiseDomainEvent(new AdminStatusChangedEvent(Id, (int)Status));
    }

    public void Deactivate()
    {
        GuardNotAlreadyInactive();
        Status = AdminStatus.Inactive;
        RaiseDomainEvent(new AdminStatusChangedEvent(Id, (int)Status));
    }

    public void Suspend()
    {
        Status = AdminStatus.Suspended;
        RaiseDomainEvent(new AdminStatusChangedEvent(Id, (int)Status));
    }

    public void ChangeRole(AdminRole newRole) => Role = newRole;

    public AuditLog LogAction(CreateAuditLogArg arg)
    {
        var log = new AuditLog(arg);
        _auditLogs.Add(log);
        return log;
    }

    public Report RequestReport(CreateReportArg arg)
    {
        var report = new Report(arg);
        _reports.Add(report);
        return report;
    }

    public void CompleteReport(long reportId, string filePath)
    {
        var report = _reports.FirstOrDefault(r => r.Id == reportId);
        GuardReportExists(report);

        report!.MarkCompleted(filePath);
        RaiseDomainEvent(new ReportGeneratedEvent(reportId, Id, (int)report.Type, filePath));
    }

    public void FailReport(long reportId, string reason)
    {
        var report = _reports.FirstOrDefault(r => r.Id == reportId);
        GuardReportExists(report);

        report!.MarkFailed(reason);
        RaiseDomainEvent(new ReportFailedEvent(reportId, reason));
    }

    private static void GuardFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new AdminInvalidNameException();
    }

    private static void GuardEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new AdminInvalidEmailException();
    }

    private void GuardNotAlreadyActive()
    {
        if (Status == AdminStatus.Active)
            throw new AdminAlreadyActiveException();
    }

    private void GuardNotAlreadyInactive()
    {
        if (Status == AdminStatus.Inactive)
            throw new AdminAlreadyInactiveException();
    }

    private static void GuardReportExists(Report? report)
    {
        if (report is null)
            throw new ReportNotFoundException();
    }
}
