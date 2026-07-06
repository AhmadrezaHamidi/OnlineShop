using Ahmad.OnlineShop.Rest.EndPoints.Identity;
using Identity.Application.Commands;
using Identity.Application.Query.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Rest.Endpoints;

public class UserEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(IdentityConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Users")
            .RequireAuthorization();

        // â”€â”€ Queries â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        group.MapGetEndpoint<GetUserQueryResponse>(
            IdentityConstants.Routes.GetUser,
            GetUser,
            IdentityConstants.Names.GetUser,
            IdentityConstants.Docs.GetUser.Summary,
            IdentityConstants.Docs.GetUser.Description);

        group.MapGetEndpoint<PagedResult<GetUserQueryResponse>>(
            IdentityConstants.Routes.GetUsers,
            GetUsers,
            IdentityConstants.Names.GetUsers,
            IdentityConstants.Docs.GetUsers.Summary,
            IdentityConstants.Docs.GetUsers.Description);

        group.MapGetEndpoint<IReadOnlyList<GetRoleQueryResponse>>(
            IdentityConstants.Routes.GetRoles,
            GetRoles,
            IdentityConstants.Names.GetRoles,
            IdentityConstants.Docs.GetRoles.Summary,
            IdentityConstants.Docs.GetRoles.Description);

        // â”€â”€ Commands â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        group.MapPutEndpoint(
            IdentityConstants.Routes.UpdateProfile,
            UpdateProfile,
            IdentityConstants.Names.UpdateProfile,
            IdentityConstants.Docs.UpdateProfile.Summary,
            IdentityConstants.Docs.UpdateProfile.Description);

        group.MapPostEndpoint(
            IdentityConstants.Routes.AssignRole,
            AssignRole,
            IdentityConstants.Names.AssignRole,
            IdentityConstants.Docs.AssignRole.Summary,
            IdentityConstants.Docs.AssignRole.Description);

        group.MapDeleteEndpoint(
            IdentityConstants.Routes.RemoveRole,
            RemoveRole,
            IdentityConstants.Names.RemoveRole,
            IdentityConstants.Docs.RemoveRole.Summary,
            IdentityConstants.Docs.RemoveRole.Description);

        group.MapPostEndpoint(
            IdentityConstants.Routes.ActivateUser,
            ActivateUser,
            IdentityConstants.Names.ActivateUser,
            IdentityConstants.Docs.ActivateUser.Summary,
            IdentityConstants.Docs.ActivateUser.Description);

        group.MapPostEndpoint(
            IdentityConstants.Routes.DeactivateUser,
            DeactivateUser,
            IdentityConstants.Names.DeactivateUser,
            IdentityConstants.Docs.DeactivateUser.Summary,
            IdentityConstants.Docs.DeactivateUser.Description);

        group.MapPostEndpoint(
            IdentityConstants.Routes.SuspendUser,
            SuspendUser,
            IdentityConstants.Names.SuspendUser,
            IdentityConstants.Docs.SuspendUser.Summary,
            IdentityConstants.Docs.SuspendUser.Description);
    }

    // â”€â”€ Query Handlers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static async Task<GetUserQueryResponse> GetUser(
        long id,
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<GetUserQueryResponse>(new GetUserQuery(id), ct);

    private static async Task<PagedResult<GetUserQueryResponse>> GetUsers(
        [AsParameters] GetUsersQuery query,
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<PagedResult<GetUserQueryResponse>>(query, ct);

    private static async Task<IReadOnlyList<GetRoleQueryResponse>> GetRoles(
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<IReadOnlyList<GetRoleQueryResponse>>(new GetRolesQuery(), ct);

    // â”€â”€ Command Handlers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static async Task<bool> UpdateProfile(
        long id,
        [FromBody] UpdateProfileCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)(command with { UserId = id }), ct);

    private static async Task<bool> AssignRole(
        long id,
        long roleId,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)new AssignRoleCommand(id, roleId), ct);

    private static async Task<bool> RemoveRole(
        long id,
        long roleId,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)new RemoveRoleCommand(id, roleId), ct);

    private static async Task<bool> ActivateUser(
        long id,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)new ActivateUserCommand(id), ct);

    private static async Task<bool> DeactivateUser(
        long id,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)new DeactivateUserCommand(id), ct);

    private static async Task<bool> SuspendUser(
        long id,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<bool>((ICommand<bool>)new SuspendUserCommand(id), ct);
}

