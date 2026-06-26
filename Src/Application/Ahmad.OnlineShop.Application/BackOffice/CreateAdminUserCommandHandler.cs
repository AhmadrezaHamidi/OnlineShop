using AhmadBase.Application;
using BackOffice.Application.Commands;
using BackOffice.Domain.Aggregates;
using BackOffice.Domain.Exceptions;
using BackOffice.Domain.Repositories;

namespace BackOffice.Application.Handlers;

public class CreateAdminUserCommandHandler : ICommandHandler<CreateAdminUserCommand, long>
{
    private readonly IAdminUserRepository _repository;

    public CreateAdminUserCommandHandler(IAdminUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<long> Handle(CreateAdminUserCommand command, CancellationToken token)
    {
        var existing = await _repository.GetByEmailAsync(command.Email, token);
        if (existing is not null)
        {
            var (code, msg) = BackOfficeErrors.Get(BackOfficeErrors.AdminInvalidEmail);
            throw new BackOfficeDomainException(code, $"Admin with email '{command.Email}' already exists.");
        }

        var id    = await _repository.GetNextIdAsync();
        var admin = AdminUser.Create(id, command.FullName, command.Email, command.Role);

        await _repository.AddAsync(admin, token);

        return admin.Id;
    }
}
