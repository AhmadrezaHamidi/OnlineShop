using Ahmad.OnlineShop.Rest.EndPoints.BackOffice;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using BackOffice.Application.Commands;
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

        group.MapGet(BackOfficeConstants.Routes.GetReports,
            async ([FromQuery] long adminId, [FromQuery] int page, [FromQuery] int pageSize,
                   IQueryBus queryBus, CancellationToken ct) =>
            {
                var result = await queryBus.DispatchAsync(new GetReportsQuery(adminId, page, pageSize), ct);
                return Results.Ok(ApiResponse<List<GetReportQueryResponse>>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.GetReports)
            .WithSummary(BackOfficeConstants.Docs.GetReports.Summary)
            .Produces<ApiResponse<List<GetReportQueryResponse>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

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
            async (long id, [FromBody] CompleteReportCommand command, ICommandBus bus, CancellationToken ct) =>
            {
                var result = await bus.Dispatch<long>((ICommand<long>)(command with { ReportId = id }), ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.CompleteReport)
            .WithSummary(BackOfficeConstants.Docs.CompleteReport.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch(BackOfficeConstants.Routes.FailReport,
            async (long id, [FromBody] FailReportCommand command, ICommandBus bus, CancellationToken ct) =>
            {
                var result = await bus.Dispatch<long>((ICommand<long>)(command with { ReportId = id }), ct);
                return Results.Ok(ApiResponse<long>.Ok(result));
            })
            .WithName(BackOfficeConstants.Names.FailReport)
            .WithSummary(BackOfficeConstants.Docs.FailReport.Summary)
            .Produces<ApiResponse<long>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
}
