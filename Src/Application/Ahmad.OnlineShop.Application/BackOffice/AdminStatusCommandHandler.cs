using AhmadBase.Application;
using BackOffice.Application.Commands;
using BackOffice.Domain.Exceptions;
using BackOffice.Domain.Repositories;

namespace BackOffice.Application.Handlers;


public class ActivateAdminCommandHandler : ICommandHandler<ActivateAdminCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public ActivateAdminCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(ActivateAdminCommand command, CancellationToken token)
    {
        var admin = await _repository.GetByIdAsync(command.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        admin.Activate();

        await _repository.UpdateAsync(admin, token);

        return admin.Id;
    }
}

// ── Deactivate ────────────────────────────────────────────────────────────────

public class DeactivateAdminCommandHandler : ICommandHandler<DeactivateAdminCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public DeactivateAdminCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(DeactivateAdminCommand command, CancellationToken token)
    {
        var admin = await _repository.GetByIdAsync(command.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        admin.Deactivate();

        await _repository.UpdateAsync(admin, token);

        return admin.Id;
    }
}

// ── Suspend ───────────────────────────────────────────────────────────────────

public class SuspendAdminCommandHandler : ICommandHandler<SuspendAdminCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public SuspendAdminCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(SuspendAdminCommand command, CancellationToken token)
    {
        var admin = await _repository.GetByIdAsync(command.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        admin.Suspend();

        await _repository.UpdateAsync(admin, token);

        return admin.Id;
    }
}
