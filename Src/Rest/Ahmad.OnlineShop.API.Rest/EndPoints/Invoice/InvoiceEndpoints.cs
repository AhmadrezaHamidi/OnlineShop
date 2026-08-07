using Ahmad.OnlineShop.Application.Query.Contract.Invoice;
using Ahmad.OnlineShop.Rest.EndPoints.Invoice;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public sealed class InvoiceEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(InvoiceConstants.Routes.BaseRoute)
            .WithTags("Invoices")
            .RequireAuthorization();

        group.MapGet(InvoiceConstants.Routes.GetById, GetById)
            .WithName(InvoiceConstants.Names.GetById)
            .WithSummary(InvoiceConstants.Docs.GetById.Summary);

        group.MapGet(InvoiceConstants.Routes.GetByOrder, GetByOrder)
            .WithName(InvoiceConstants.Names.GetByOrder)
            .WithSummary(InvoiceConstants.Docs.GetByOrder.Summary);

        group.MapGet(InvoiceConstants.Routes.GetMyInvoices, GetMyInvoices)
            .WithName(InvoiceConstants.Names.GetMyInvoices)
            .WithSummary(InvoiceConstants.Docs.GetMyInvoices.Summary);
    }

    private static async Task<IResult> GetById(
        long id, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetInvoiceResponse?>(
            new GetInvoiceByIdQuery(id), ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<GetInvoiceResponse>.Ok(result));
    }

    private static async Task<IResult> GetByOrder(
        long orderId, IQueryBus queryBus, CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetInvoiceResponse?>(
            new GetInvoiceByOrderIdQuery(orderId), ct);
        return result is null
            ? Results.NotFound()
            : Results.Ok(ApiResponse<GetInvoiceResponse>.Ok(result));
    }

    private static async Task<IResult> GetMyInvoices(
        long userId, IQueryBus queryBus, CancellationToken ct,
        int page = 1, int pageSize = 20)
    {
        var result = await queryBus.DispatchAsync<PagedResult<GetInvoiceResponse>>(
            new GetMyInvoicesQuery(userId, page, pageSize), ct);
        return Results.Ok(ApiResponse<PagedResult<GetInvoiceResponse>>.Ok(result));
    }
}
