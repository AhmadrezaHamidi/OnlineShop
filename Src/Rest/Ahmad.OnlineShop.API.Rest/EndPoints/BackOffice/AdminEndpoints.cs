using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using Ahmad.OnlineShop.Rest.EndPoints.BackOffice;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using BackOffice.Application.Commands;
using BackOffice.Application.Dtos;
using BackOffice.Application.Query.Queries;
using BackOffice.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BackOffice.Rest.Endpoints;

public class AdminEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BackOfficeConstants.Routes.BaseRoute)
            .WithTags("BackOffice - Admins")
            .RequireAuthorization();

        // ── Queries ───────────────────────────────────────────────────────────
        group.MapGet(BackOfficeConstants.Routes.GetAdmin,
            async (long id, IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(new GetAdminUserQuery(id), ct);
                return Results.Ok(ApiResponse<AdminUserDto>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.GetAdmin)
            .WithSummary(BackOfficeConstants.Docs.GetAdmin.Summary)
            .Produces<ApiResponse<AdminUserDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet(BackOfficeConstants.Routes.GetAdmins,
            async ([AsParameters] GetAdminUsersQuery query, IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(query, ct);
                return Results.Ok(ApiResponse<PagedResult<AdminUserDto>>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.GetAdmins)
            .WithSummary(BackOfficeConstants.Docs.GetAdmins.Summary)
            .Produces<ApiResponse<PagedResult<AdminUserDto>>>(StatusCodes.Status200OK);

        group.MapGet(BackOfficeConstants.Routes.GetAuditLogs,
            async (long id, [FromQuery] int page, [FromQuery] int pageSize,
                   IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(new GetAuditLogsQuery(id, page, pageSize), ct);
                return Results.Ok(ApiResponse<List<AuditLogDto>>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.GetAuditLogs)
            .WithSummary(BackOfficeConstants.Docs.GetAuditLogs.Summary)
            .Produces<ApiResponse<List<AuditLogDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // ── Commands ──────────────────────────────────────────────────────────
        group.MapPost(BackOfficeConstants.Routes.CreateAdmin,
            async ([FromBody] CreateAdminUserCommand command, ICommandBus bus, CancellationToken ct) =>
            {
                var result = await bus.Dispatch<long>((ICommand<long>)command, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.CreateAdmin)
            .WithSummary(BackOfficeConstants.Docs.CreateAdmin.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK);

        group.MapPatch(BackOfficeConstants.Routes.ActivateAdmin,
            async (long id, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd = new ActivateAdminCommand(id);
                var result = await bus.Dispatch<long>((ICommand<long>)cmd, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.ActivateAdmin)
            .WithSummary(BackOfficeConstants.Docs.ActivateAdmin.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch(BackOfficeConstants.Routes.DeactivateAdmin,
            async (long id, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd = new DeactivateAdminCommand(id);
                var result = await bus.Dispatch<long>((ICommand<long>)cmd, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.DeactivateAdmin)
            .WithSummary(BackOfficeConstants.Docs.DeactivateAdmin.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch(BackOfficeConstants.Routes.SuspendAdmin,
            async (long id, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd = new SuspendAdminCommand(id);
                var result = await bus.Dispatch<long>((ICommand<long>)cmd, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.SuspendAdmin)
            .WithSummary(BackOfficeConstants.Docs.SuspendAdmin.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch(BackOfficeConstants.Routes.ChangeAdminRole,
            async (long id, [FromBody] ChangeAdminRoleRequest req, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd = new ChangeAdminRoleCommand(id, req.NewRole);
                var result = await bus.Dispatch<long>((ICommand<long>)cmd, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.ChangeAdminRole)
            .WithSummary(BackOfficeConstants.Docs.ChangeAdminRole.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}

// ── Request models ─────────────────────────────────────────────────────────────
public record ChangeAdminRoleRequest(AdminRole NewRole);