using Ahmad.OnlineShop.Rest.EndPoints.BackOffice;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using BackOffice.Application.Commands;
using BackOffice.Application.Dtos;
using BackOffice.Application.Query.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BackOffice.Rest.Endpoints;

public class ReportEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BackOfficeConstants.Routes.BaseRoute)
            .WithTags("BackOffice - Reports")
            .RequireAuthorization();

        // ── Queries ───────────────────────────────────────────────────────────
        group.MapGet(BackOfficeConstants.Routes.GetReports,
            async ([FromQuery] long adminId, [FromQuery] int page, [FromQuery] int pageSize,
                   IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(new GetReportsQuery(adminId, page, pageSize), ct);
                return Results.Ok(ApiResponse<List<ReportDto>>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.GetReports)
            .WithSummary(BackOfficeConstants.Docs.GetReports.Summary)
            .Produces<ApiResponse<List<ReportDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // ── Commands ──────────────────────────────────────────────────────────
        group.MapPost(BackOfficeConstants.Routes.RequestReport,
            async ([FromBody] RequestReportCommand command, ICommandBus bus, CancellationToken ct) =>
            {
                var result = await bus.Dispatch<long>((ICommand<long>)command, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.RequestReport)
            .WithSummary(BackOfficeConstants.Docs.RequestReport.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch(BackOfficeConstants.Routes.CompleteReport,
            async (long id, [FromBody] CompleteReportRequest req, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd = new CompleteReportCommand(req.AdminId, id, req.FilePath);
                var result = await bus.Dispatch<long>((ICommand<long>)cmd, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.CompleteReport)
            .WithSummary(BackOfficeConstants.Docs.CompleteReport.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch(BackOfficeConstants.Routes.FailReport,
            async (long id, [FromBody] FailReportRequest req, ICommandBus bus, CancellationToken ct) =>
            {
                var cmd = new FailReportCommand(req.AdminId, id, req.Reason);
                var result = await bus.Dispatch<long>((ICommand<long>)cmd, ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.FailReport)
            .WithSummary(BackOfficeConstants.Docs.FailReport.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}

// ── Request models ─────────────────────────────────────────────────────────────
public record CompleteReportRequest(long AdminId, string FilePath);
public record FailReportRequest(long AdminId, string Reason);