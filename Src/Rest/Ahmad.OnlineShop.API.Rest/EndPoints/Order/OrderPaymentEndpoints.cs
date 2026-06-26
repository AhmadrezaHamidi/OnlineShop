using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Rest.EndPoints.Order;
using AhmadBase.Application;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public sealed class OrderPaymentEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(OrderConstants.Routes.BaseRoute)
            .WithTags("Order.Payments");

        group.MapPost(OrderConstants.Routes.RecordPayment, RecordPayment)
            .WithName(OrderConstants.Names.RecordPayment)
            .WithSummary(OrderConstants.Docs.RecordPayment.Summary);

        group.MapPatch(OrderConstants.Routes.CompletePayment, ["PATCH"], CompletePayment)
            .WithName(OrderConstants.Names.CompletePayment)
            .WithSummary(OrderConstants.Docs.CompletePayment.Summary);

        group.MapMethods(OrderConstants.Routes.FailPayment, ["PATCH"], FailPayment)
            .WithName(OrderConstants.Names.FailPayment)
            .WithSummary(OrderConstants.Docs.FailPayment.Summary);
    }

    // ── Handlers ─────────────────────────────────────────────────────

    private static async Task<IResult> RecordPayment(
        long id,
        RecordPaymentRequest body,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(
            new RecordPaymentCommand(id, body.PaymentId, body.Amount, body.Provider), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> CompletePayment(
        long id,
        long paymentId,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new CompletePaymentCommand(id, paymentId), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> FailPayment(
        long id,
        long paymentId,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new FailPaymentCommand(id, paymentId), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }
}

public sealed record RecordPaymentRequest(
    long PaymentId,
    decimal Amount,
    string? Provider = null
);