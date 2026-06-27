using Ahmad.OnlineShop.Application.BackOffice.Mapper;
using Ahmad.OnlineShop.Domain.BackOffice.Aggregates;
using Ahmad.OnlineShop.Domain.BackOffice.Args;
using Ahmad.OnlineShop.Domain.BackOffice.Exceptions;
using BackOffice.Application.Commands;
using BackOffice.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.BackOffice;

public class BackOfficeHandlers(IAdminUserRepository repository) :
    ICommandHandler<CreateAdminUserCommand, long>,
    ICommandHandler<ChangeAdminRoleCommand, long>,
    ICommandHandler<ActivateAdminCommand, long>,
    ICommandHandler<DeactivateAdminCommand, long>,
    ICommandHandler<SuspendAdminCommand, long>,
    ICommandHandler<RequestReportCommand, long>,
    ICommandHandler<CompleteReportCommand, long>,
    ICommandHandler<FailReportCommand, long>
{
    #region Admin Commands

    public async Task<long> Handle(CreateAdminUserCommand command, CancellationToken token)
    {
        var existing = await repository.GetByEmailAsync(command.Email, token);
        if (existing is not null)
            throw new AdminEmailAlreadyExistsException();

        var id = repository.GetNextId();
        var admin = AdminUser.Create(command.Map(id));

        await repository.AddAsync(admin, token);

        return admin.Id;
    }

    public async Task<long> Handle(ChangeAdminRoleCommand command, CancellationToken token)
    {
        var admin = await repository.GetByIdAsync(command.AdminId, token)
            ?? throw new AdminNotFoundException();

        admin.ChangeRole(command.NewRole);

        await repository.UpdateAsync(admin, token);

        return admin.Id;
    }

    #endregion

    #region Admin Status Commands

    public async Task<long> Handle(ActivateAdminCommand command, CancellationToken token)
    {
        var admin = await repository.GetByIdAsync(command.AdminId, token)
            ?? throw new AdminNotFoundException();

        admin.Activate();

        await repository.UpdateAsync(admin, token);

        return admin.Id;
    }

    public async Task<long> Handle(DeactivateAdminCommand command, CancellationToken token)
    {
        var admin = await repository.GetByIdAsync(command.AdminId, token)
            ?? throw new AdminNotFoundException();

        admin.Deactivate();

        await repository.UpdateAsync(admin, token);

        return admin.Id;
    }

    public async Task<long> Handle(SuspendAdminCommand command, CancellationToken token)
    {
        var admin = await repository.GetByIdAsync(command.AdminId, token)
            ?? throw new AdminNotFoundException();

        admin.Suspend();

        await repository.UpdateAsync(admin, token);

        return admin.Id;
    }

    #endregion

    #region Report Commands

    public async Task<long> Handle(RequestReportCommand command, CancellationToken token)
    {
        var admin = await repository.GetByIdAsync(command.AdminId, token)
            ?? throw new AdminNotFoundException();

        var report = admin.RequestReport(new CreateReportArg(command.ReportId, admin.Id, command.Type));

        await repository.UpdateAsync(admin, token);

        return report.Id;
    }

    public async Task<long> Handle(CompleteReportCommand command, CancellationToken token)
    {
        var admin = await repository.GetByIdAsync(command.AdminId, token)
            ?? throw new AdminNotFoundException();

        admin.CompleteReport(command.ReportId, command.FilePath);

        await repository.UpdateAsync(admin, token);

        return command.ReportId;
    }

    public async Task<long> Handle(FailReportCommand command, CancellationToken token)
    {
        var admin = await repository.GetByIdAsync(command.AdminId, token)
            ?? throw new AdminNotFoundException();

        admin.FailReport(command.ReportId, command.Reason);

        await repository.UpdateAsync(admin, token);

        return command.ReportId;
    }

    #endregion
}
