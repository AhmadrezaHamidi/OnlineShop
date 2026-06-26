using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Application.Services;
using Identity.Domain.Aggregates;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, long>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher  _passwordHasher;

    public RegisterCommandHandler(IUserRepository repo, IPasswordHasher passwordHasher)
    {
        _repo           = repo;
        _passwordHasher = passwordHasher;
    }

    public async Task<long> Handle(RegisterCommand command, CancellationToken token)
    {
        var exists = await _repo.ExistsByEmailAsync(command.Email, token);
        if (exists)
        {
            var (code, msg) = UserErrors.Get(UserErrors.DuplicateEmail);
            throw new UserDomainException(code, msg);
        }

        var id           = await _repo.GetNextIdAsync();
        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = User.Register(id, command.FullName, command.Email, passwordHash, command.PhoneNumber);

        await _repo.AddAsync(user, token);

        return user.Id;
    }
}
