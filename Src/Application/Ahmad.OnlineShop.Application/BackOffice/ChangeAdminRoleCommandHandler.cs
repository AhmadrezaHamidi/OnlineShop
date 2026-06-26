using AhmadBase.Application;
using BackOffice.Application.Commands;
using BackOffice.Domain.Exceptions;
using BackOffice.Domain.Repositories;

namespace BackOffice.Application.Handlers;

public class ChangeAdminRoleCommandHandler : ICommandHandler<ChangeAdminRoleCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public ChangeAdminRoleCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(ChangeAdminRoleCommand command, CancellationToken token)
    {
        var admin = await _repository.GetByIdAsync(command.AdminId, token);
        if (admin is null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminNotFound);
            throw new BackOfficeDomainException(code, msg);
        }

        admin.ChangeRole(command.NewRole);

        await _repository.UpdateAsync(admin, token);

        return admin.Id;
    }
}
