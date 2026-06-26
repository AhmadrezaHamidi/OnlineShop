using AhmadBase.Application;
using BackOffice.Application.Commands;
using BackOffice.Domain.Exceptions;
using BackOffice.Domain.Repositories;

namespace BackOffice.Application.Handlers;

// ── Request ───────────────────────────────────────────────────────────────────

public class RequestReportCommandHandler : ICommandHandler<RequestReportCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public RequestReportCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(RequestReportCommand command, CancellationToken token)
    {
        var admin = await _repository.GetByIdAsync(command.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        var report = admin.RequestReport(command.ReportId, command.Type);

        await _repository.UpdateAsync(admin, token);

        return report.Id;
    }
}

// ── Complete ──────────────────────────────────────────────────────────────────

public class CompleteReportCommandHandler : ICommandHandler<CompleteReportCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public CompleteReportCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(CompleteReportCommand command, CancellationToken token)
    {
        var admin = await _repository.GetByIdAsync(command.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        admin.CompleteReport(command.ReportId, command.FilePath);

        await _repository.UpdateAsync(admin, token);

        return command.ReportId;
    }
}

// ── Fail ──────────────────────────────────────────────────────────────────────

public class FailReportCommandHandler : ICommandHandler<FailReportCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public FailReportCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(FailReportCommand command, CancellationToken token)
    {
        var admin = await _repository.GetByIdAsync(command.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        admin.FailReport(command.ReportId, command.Reason);

        await _repository.UpdateAsync(admin, token);

        return command.ReportId;
    }
}
