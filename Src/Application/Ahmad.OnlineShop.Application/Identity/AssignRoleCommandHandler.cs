using AhmadBase.Application;
using Identity.Application.Commands;
using Identity.Domain.Repositories;

namespace Identity.Application.Handlers;

public class AssignRoleCommandHandler : ICommandHandler<AssignRoleCommand, bool>
{
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;

    public AssignRoleCommandHandler(IUserRepository userRepo, IRoleRepository roleRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
    }

    public async Task<bool> Handle(AssignRoleCommand command, CancellationToken token)
    {
        var user = await _userRepo.GetByIdAsync(command.UserId, token);
        if (user is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.NotFound);
            throw new UserDomainException(code, msg);
        }

        var role = await _roleRepo.GetByIdAsync(command.RoleId, token);
        if (role is null)
        {
            var (code, msg) = UserErrors.Get(UserErrors.RoleNotFound);
            throw new UserDomainException(code, msg);
        }

        user.AssignRole(command.RoleId);

        await _userRepo.UpdateAsync(user, token);
        return true;
    }
}
