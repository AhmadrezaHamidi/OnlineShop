using Microsoft.AspNetCore.Mvc;
using Ahmad.OnlineShop.Application.Contract.Order.Commands;
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

        group.MapPatch(OrderConstants.Routes.CompletePayment, CompletePayment)
            .WithName(OrderConstants.Names.CompletePayment)
            .WithSummary(OrderConstants.Docs.CompletePayment.Summary);

        group.MapPatch(OrderConstants.Routes.FailPayment, FailPayment)
            .WithName(OrderConstants.Names.FailPayment)
            .WithSummary(OrderConstants.Docs.FailPayment.Summary);

        // ── درگاه پرداخت آنلاین ─────────────────────────────────────────────
        group.MapPost("{id:long}/payment/initiate", InitiateOnlinePayment)
            .WithName("Order.InitiateOnlinePayment")
            .WithSummary("شروع پرداخت آنلاین (ZarinPal). اگر درگاه در دسترس نباشد، به CashOnDelivery fallback می‌شود.");

        group.MapGet("{id:long}/payment/verify", VerifyOnlinePayment)
            .WithName("Order.VerifyOnlinePayment")
            .WithSummary("تأیید پرداخت ZarinPal پس از بازگشت از درگاه");
    }

    private static async Task<IResult> RecordPayment(
        long id,
        [FromBody] RecordPaymentCommand command,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(command with { OrderId = id }, ct);
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

    private static async Task<IResult> InitiateOnlinePayment(
        long id,
        [FromBody] InitiateOnlinePaymentRequest request,
        ICommandBus bus,
        CancellationToken ct)
    {
        var command = new InitiateOnlinePaymentCommand(id, request.CallbackUrl, request.Description ?? "پرداخت سفارش");
        var result  = await bus.Dispatch<InitiatePaymentResult>(command, ct);
        return Results.Ok(ApiResponse<InitiatePaymentResult>.Ok(result));
    }

    private static async Task<IResult> VerifyOnlinePayment(
        long id,
        long paymentId,
        string authority,
        string status,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<bool>(new VerifyOnlinePaymentCommand(id, paymentId, authority, status), ct);
        return Results.Ok(ApiResponse<bool>.Ok(result));
    }

    public sealed record InitiateOnlinePaymentRequest(string CallbackUrl, string? Description);
}
