using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.EndPoints.Product;

public sealed class ProductInventoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Product.Inventory");

        group.MapGetEndpoint(
            ProductConstants.Routes.GetInventory,
            GetInventory,
            ProductConstants.Names.GetInventory,
            ProductConstants.Docs.GetInventory.Summary,
            ProductConstants.Docs.GetInventory.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReplenishStock,
            PatchReplenishStock,
            ProductConstants.Names.ReplenishStock,
            ProductConstants.Docs.ReplenishStock.Summary,
            ProductConstants.Docs.ReplenishStock.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReserveStock,
            PatchReserveStock,
            ProductConstants.Names.ReserveStock,
            ProductConstants.Docs.ReserveStock.Summary,
            ProductConstants.Docs.ReserveStock.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReleaseStock,
            PatchReleaseStock,
            ProductConstants.Names.ReleaseStock,
            ProductConstants.Docs.ReleaseStock.Summary,
            ProductConstants.Docs.ReleaseStock.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ConfirmStock,
            PatchConfirmStock,
            ProductConstants.Names.ConfirmStock,
            ProductConstants.Docs.ConfirmStock.Summary,
            ProductConstants.Docs.ConfirmStock.Description);
    }

    // ─── Handlers ─────────────────────────────────────────────────────────────
    private static async Task<GetInventoryQueryResponse> GetInventory(
        long id,
        IQueryBus queryBus,
        CancellationToken cancellation)
        => await queryBus.DispatchAsync<GetInventoryQueryResponse>(new GetInventoryQuery(id), cancellation);

    private static async Task<long> PatchReplenishStock(
        long id,
        ReplenishStockCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command with { ProductId = id }, cancellation);

    private static async Task<long> PatchReserveStock(
        long id,
        ReserveStockCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command with { ProductId = id }, cancellation);

    private static async Task<long> PatchReleaseStock(
        long id,
        ReleaseStockCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command with { ProductId = id }, cancellation);

    private static async Task<long> PatchConfirmStock(
        long id,
        ConfirmStockCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<long>(command with { ProductId = id }, cancellation);
}