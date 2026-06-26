using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Application.Services;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, bool>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher  _passwordHasher;

    public ChangePasswordCommandHandler(IUserRepository repo, IPasswordHasher passwordHasher)
    {
        _repo           = repo;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> Handle(ChangePasswordCommand command, CancellationToken token)
    {
        var user = await _repo.GetByIdAsync(command.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        var currentValid = _passwordHasher.Verify(command.CurrentPassword, user.PasswordHash);
        if (!currentValid)
        {
            var (code, msg) = UserErrors.Get(UserErrors.InvalidPasswordHash);
            throw new UserDomainException(code, msg);
        }

        var newHash = _passwordHasher.Hash(command.NewPassword);
        user.ChangePassword(newHash);

        await _repo.UpdateAsync(user, token);
        return true;
    }
}
