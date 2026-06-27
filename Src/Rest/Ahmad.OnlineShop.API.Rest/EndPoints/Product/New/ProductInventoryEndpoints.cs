using Ahmad.OnlineShop.Rest.EndPoints.Product;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public class ProductInventoryEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Product Inventory");

        group.MapGetEndpoint<GetProductInventoryResponse>(
            ProductConstants.Routes.GetInventory,
            GetInventory,
            ProductConstants.Names.GetInventory,
            ProductConstants.Docs.GetInventory.Summary,
            ProductConstants.Docs.GetInventory.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReplenishStock,
            ReplenishStock,
            ProductConstants.Names.ReplenishStock,
            ProductConstants.Docs.ReplenishStock.Summary,
            ProductConstants.Docs.ReplenishStock.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReserveStock,
            ReserveStock,
            ProductConstants.Names.ReserveStock,
            ProductConstants.Docs.ReserveStock.Summary,
            ProductConstants.Docs.ReserveStock.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReleaseStock,
            ReleaseStock,
            ProductConstants.Names.ReleaseStock,
            ProductConstants.Docs.ReleaseStock.Summary,
            ProductConstants.Docs.ReleaseStock.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ConfirmStock,
            ConfirmStock,
            ProductConstants.Names.ConfirmStock,
            ProductConstants.Docs.ConfirmStock.Summary,
            ProductConstants.Docs.ConfirmStock.Description);
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<GetProductInventoryResponse> GetInventory(
        long id,
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<GetProductInventoryResponse>(new GetInventoryQuery(id), ct);

    private static async Task<long> ReplenishStock(
        long id,
        ReplenishStockCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { ProductId = id }, ct);

    private static async Task<long> ReserveStock(
        long id,
        ReserveStockCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { ProductId = id }, ct);

    private static async Task<long> ReleaseStock(
        long id,
        ReleaseStockCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { ProductId = id }, ct);

    private static async Task<long> ConfirmStock(
        long id,
        ConfirmStockCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { ProductId = id }, ct);
}
