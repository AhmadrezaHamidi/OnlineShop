using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, bool>
{
    private readonly IUserRepository _repo;

    public UpdateProfileCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(UpdateProfileCommand command, CancellationToken token)
    {
        var user = await _repo.GetByIdAsync(command.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        user.UpdateProfile(command.FullName, command.PhoneNumber);

        await _repo.UpdateAsync(user, token);
        return true;
    }
}
