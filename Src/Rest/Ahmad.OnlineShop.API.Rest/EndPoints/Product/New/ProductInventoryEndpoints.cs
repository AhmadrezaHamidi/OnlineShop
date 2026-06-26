using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public class ProductInventoryEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products/{productId:long}/inventory")
                       .WithTags("Product Inventory");

        // GET /api/products/{productId}/inventory
        group.MapGet("/", async (
            long              productId,
            IQueryBus         queryBus,
            CancellationToken token = default) =>
        {
            var result = await queryBus.Dispatch<InventoryDto>(
                new GetInventoryQuery(productId), token);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetInventory")
        .WithSummary("Get inventory for a product")
        .Produces<InventoryDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{productId}/inventory/reserve
        group.MapPatch("/reserve", async (
            long                  productId,
            StockQuantityRequest  req,
            ICommandBus           bus,
            CancellationToken     token = default) =>
        {
            await bus.Dispatch<long>(new ReserveStockCommand(productId, req.Quantity), token);
            return Results.NoContent();
        })
        .WithName("ReserveStock")
        .WithSummary("Reserve stock for a product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{productId}/inventory/release
        group.MapPatch("/release", async (
            long                  productId,
            StockQuantityRequest  req,
            ICommandBus           bus,
            CancellationToken     token = default) =>
        {
            await bus.Dispatch<long>(new ReleaseStockCommand(productId, req.Quantity), token);
            return Results.NoContent();
        })
        .WithName("ReleaseStock")
        .WithSummary("Release reserved stock for a product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{productId}/inventory/confirm
        group.MapPatch("/confirm", async (
            long                  productId,
            StockQuantityRequest  req,
            ICommandBus           bus,
            CancellationToken     token = default) =>
        {
            await bus.Dispatch<long>(new ConfirmStockCommand(productId, req.Quantity), token);
            return Results.NoContent();
        })
        .WithName("ConfirmStock")
        .WithSummary("Confirm (deduct) reserved stock after order completion")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{productId}/inventory/replenish
        group.MapPatch("/replenish", async (
            long                  productId,
            StockQuantityRequest  req,
            ICommandBus           bus,
            CancellationToken     token = default) =>
        {
            await bus.Dispatch<long>(new ReplenishStockCommand(productId, req.Quantity), token);
            return Results.NoContent();
        })
        .WithName("ReplenishStock")
        .WithSummary("Replenish (add) stock to a product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }
}

// ── request body records ──────────────────────────────────────────────────────

public record StockQuantityRequest(int Quantity);
