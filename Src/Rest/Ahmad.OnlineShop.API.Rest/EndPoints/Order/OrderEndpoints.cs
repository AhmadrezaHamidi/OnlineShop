using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Application.Contract.Order.Commands;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Rest.EndPoints.Order;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using AhmadBase.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public sealed class OrderEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(OrderConstants.Routes.BaseRoute)
            .WithTags("Orders");

        // ── Commands ────────────────────────────────────────────────────
        group.MapPost(OrderConstants.Routes.CreateOrder, CreateOrder)
            .WithName(OrderConstants.Names.CreateOrder)
            .WithSummary(OrderConstants.Docs.CreateOrder.Summary);

        group.MapPost(OrderConstants.Routes.AddItem, AddItem)
            .WithName(OrderConstants.Names.AddItem)
            .WithSummary(OrderConstants.Docs.AddItem.Summary);

        group.MapDelete(OrderConstants.Routes.RemoveItem, RemoveItem)
            .WithName(OrderConstants.Names.RemoveItem)
            .WithSummary(OrderConstants.Docs.RemoveItem.Summary);

        group.MapPost(OrderConstants.Routes.PlaceOrder, PlaceOrder)
            .WithName(OrderConstants.Names.PlaceOrder)
            .WithSummary(OrderConstants.Docs.PlaceOrder.Summary);

        group.MapPost(OrderConstants.Routes.ConfirmOrder, ConfirmOrder)
            .WithName(OrderConstants.Names.ConfirmOrder)
            .WithSummary(OrderConstants.Docs.ConfirmOrder.Summary);

        group.MapPost(OrderConstants.Routes.ShipOrder, ShipOrder)
            .WithName(OrderConstants.Names.ShipOrder)
            .WithSummary(OrderConstants.Docs.ShipOrder.Summary);

        group.MapPost(OrderConstants.Routes.DeliverOrder, DeliverOrder)
            .WithName(OrderConstants.Names.DeliverOrder)
            .WithSummary(OrderConstants.Docs.DeliverOrder.Summary);

        group.MapPost(OrderConstants.Routes.CancelOrder, CancelOrder)
            .WithName(OrderConstants.Names.CancelOrder)
            .WithSummary(OrderConstants.Docs.CancelOrder.Summary);

        // ── Queries ─────────────────────────────────────────────────────
        group.MapGet(OrderConstants.Routes.GetOrders, GetOrders)
            .WithName(OrderConstants.Names.GetOrders)
            .WithSummary(OrderConstants.Docs.GetOrders.Summary);

        group.MapGet(OrderConstants.Routes.GetOrder, GetOrder)
            .WithName(OrderConstants.Names.GetOrder)
            .WithSummary(OrderConstants.Docs.GetOrder.Summary);
    }

    // ── Command Handlers ─────────────────────────────────────────────
    private static async Task<IResult> CreateOrder(
        CreateOrderCommand command,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(command, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> AddItem(
        long id,
        AddOrderItemCommand command,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(command with { OrderId = id }, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> RemoveItem(
        long id,
        long itemId,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new RemoveOrderItemCommand(id, itemId), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> PlaceOrder(
        long id,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new PlaceOrderCommand(id), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> ConfirmOrder(
        long id,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new ConfirmOrderCommand(id), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> ShipOrder(
        long id,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new ShipOrderCommand(id), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> DeliverOrder(
        long id,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(new DeliverOrderCommand(id), ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    private static async Task<IResult> CancelOrder(
        long id,
        CancelOrderCommand command,
        ICommandBus bus,
        CancellationToken ct)
    {
        var result = await bus.Dispatch<long>(command with { OrderId = id }, ct);
        return Results.Ok(ApiResponse<long>.Ok(result));
    }

    // ── Query Handlers ───────────────────────────────────────────────
    private static async Task<IResult> GetOrders(
        IQueryBus queryBus,
        CancellationToken ct,
        int page = 1,
        int pageSize = 20,
        long? userId = null,
        OrderStatus? status = null)
    {
        var result = await queryBus.DispatchAsync<QueryPagedResult<GetOrderQueryResponse>>(
            new GetOrdersQuery(page, pageSize, userId, status), ct);
        return Results.Ok(ApiResponse<QueryPagedResult<GetOrderQueryResponse>>.Ok(result));
    }

    private static async Task<IResult> GetOrder(
        long id,
        IQueryBus queryBus,
        CancellationToken ct)
    {
        var result = await queryBus.DispatchAsync<GetOrderQueryResponse>(new GetOrderQuery(id), ct);
        return Results.Ok(ApiResponse<GetOrderQueryResponse>.Ok(result));
    }
}
