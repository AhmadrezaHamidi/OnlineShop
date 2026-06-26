using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Identity.Application.Commands;
using Identity.Application.Dtos;
using Identity.Application.Query.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Identity.Rest.Endpoints;

public class UserEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
            .WithTags("Users")
            .RequireAuthorization();

        // ── Queries ───────────────────────────────────────────────────────────

        group.MapGet("/{userId:long}",
            async (long userId, IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(new GetUserQuery(userId), ct);
                return Results.Ok(ApiResponse<UserDto>.Ok(result));
            })
            .WithName("Users.GetById")
            .WithSummary("Get a user by ID")
            .Produces<ApiResponse<UserDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/",
            async ([AsParameters] GetUsersQuery query, IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(query, ct);
                return Results.Ok(ApiResponse<PagedResult<UserDto>>.Ok(result));
            })
            .WithName("Users.GetList")
            .WithSummary("Get a paged list of users")
            .Produces<ApiResponse<PagedResult<UserDto>>>(StatusCodes.Status200OK);

        group.MapGet("/roles",
            async (IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(new GetRolesQuery(), ct);
                return Results.Ok(ApiResponse<IReadOnlyList<RoleDto>>.Ok(result));
            })
            .WithName("Users.GetRoles")
            .WithSummary("Get all available roles")
            .Produces<ApiResponse<IReadOnlyList<RoleDto>>>(StatusCodes.Status200OK);

        // ── Commands ──────────────────────────────────────────────────────────

        group.MapPut("/{userId:long}/profile",
            async (long userId, [FromBody] UpdateProfileRequest req, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd    = new UpdateProfileCommand(userId, req.FullName, req.PhoneNumber);
                var result = await bus.Dispatch<bool>((ICommand<bool>)cmd, ct);
                return Results.Ok(ApiResponse<bool>.Ok(result));
            })
            .WithName("Users.UpdateProfile")
            .WithSummary("Update user profile")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{userId:long}/roles/{roleId:long}",
            async (long userId, long roleId, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd    = new AssignRoleCommand(userId, roleId);
                var result = await bus.Dispatch<bool>((ICommand<bool>)cmd, ct);
                return Results.Ok(ApiResponse<bool>.Ok(result));
            })
            .WithName("Users.AssignRole")
            .WithSummary("Assign a role to a user")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{userId:long}/roles/{roleId:long}",
            async (long userId, long roleId, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd    = new RemoveRoleCommand(userId, roleId);
                var result = await bus.Dispatch<bool>((ICommand<bool>)cmd, ct);
                return Results.Ok(ApiResponse<bool>.Ok(result));
            })
            .WithName("Users.RemoveRole")
            .WithSummary("Remove a role from a user")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{userId:long}/activate",
            async (long userId, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd    = new ActivateUserCommand(userId);
                var result = await bus.Dispatch<bool>((ICommand<bool>)cmd, ct);
                return Results.Ok(ApiResponse<bool>.Ok(result));
            })
            .WithName("Users.Activate")
            .WithSummary("Activate a user account")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{userId:long}/deactivate",
            async (long userId, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd    = new DeactivateUserCommand(userId);
                var result = await bus.Dispatch<bool>((ICommand<bool>)cmd, ct);
                return Results.Ok(ApiResponse<bool>.Ok(result));
            })
            .WithName("Users.Deactivate")
            .WithSummary("Deactivate a user account")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{userId:long}/suspend",
            async (long userId, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd    = new SuspendUserCommand(userId);
                var result = await bus.Dispatch<bool>((ICommand<bool>)cmd, ct);
                return Results.Ok(ApiResponse<bool>.Ok(result));
            })
            .WithName("Users.Suspend")
            .WithSummary("Suspend a user account")
            .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}

// ── Request models ─────────────────────────────────────────────────────────────

/// <summary>Body payload for the update-profile endpoint.</summary>
public record UpdateProfileRequest(string FullName, string? PhoneNumber);
