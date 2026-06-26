using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

// ── Activate ──────────────────────────────────────────────────────────────────

public class ActivateUserCommandHandler : ICommandHandler<ActivateUserCommand, bool>
{
    private readonly IUserRepository _repo;

    public ActivateUserCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(ActivateUserCommand command, CancellationToken token)
    {
        var user = await _repo.GetByIdAsync(command.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        user.Activate();

        await _repo.UpdateAsync(user, token);
        return true;
    }
}

// ── Deactivate ────────────────────────────────────────────────────────────────

public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand, bool>
{
    private readonly IUserRepository _repo;

    public DeactivateUserCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(DeactivateUserCommand command, CancellationToken token)
    {
        var user = await _repo.GetByIdAsync(command.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        user.Deactivate();

        await _repo.UpdateAsync(user, token);
        return true;
    }
}

// ── Suspend ───────────────────────────────────────────────────────────────────

public class SuspendUserCommandHandler : ICommandHandler<SuspendUserCommand, bool>
{
    private readonly IUserRepository _repo;

    public SuspendUserCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(SuspendUserCommand command, CancellationToken token)
    {
        var user = await _repo.GetByIdAsync(command.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        user.Suspend();

        await _repo.UpdateAsync(user, token);
        return true;
    }
}
