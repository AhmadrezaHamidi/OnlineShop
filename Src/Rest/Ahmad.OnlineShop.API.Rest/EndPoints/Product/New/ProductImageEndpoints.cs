using Ahmad.OnlineShop.Rest.EndPoints.Product;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public class ProductImageEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Product Images");

        group.MapGetEndpoint<List<GetProductImageResponse>>(
            ProductConstants.Routes.GetImages,
            GetImages,
            ProductConstants.Names.GetImages,
            ProductConstants.Docs.GetImages.Summary,
            ProductConstants.Docs.GetImages.Description);

        group.MapPostEndpoint(
            ProductConstants.Routes.AddImage,
            AddImage,
            ProductConstants.Names.AddImage,
            ProductConstants.Docs.AddImage.Summary,
            ProductConstants.Docs.AddImage.Description);

        group.MapDeleteEndpoint(
            ProductConstants.Routes.RemoveImage,
            RemoveImage,
            ProductConstants.Names.RemoveImage,
            ProductConstants.Docs.RemoveImage.Summary,
            ProductConstants.Docs.RemoveImage.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.SetPrimaryImage,
            SetPrimaryImage,
            ProductConstants.Names.SetPrimaryImage,
            ProductConstants.Docs.SetPrimaryImage.Summary,
            ProductConstants.Docs.SetPrimaryImage.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReorderImage,
            ReorderImage,
            ProductConstants.Names.ReorderImage,
            ProductConstants.Docs.ReorderImage.Summary,
            ProductConstants.Docs.ReorderImage.Description);
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<List<GetProductImageResponse>> GetImages(
        long id,
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<List<GetProductImageResponse>>(
            new GetProductImagesQuery(id), ct);

    private static async Task<long> AddImage(
        long id,
        AddProductImageCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { ProductId = id }, ct);

    private static async Task<Guid> RemoveImage(
        long id,
        Guid imageId,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<Guid>(new RemoveProductImageCommand(id, imageId), ct);

    private static async Task<Guid> SetPrimaryImage(
        long id,
        Guid imageId,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<Guid>(new SetPrimaryImageCommand(id, imageId), ct);

    private static async Task<Guid> ReorderImage(
        long id,
        Guid imageId,
        ReorderProductImageCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<Guid>(command with { ProductId = id, ImageId = imageId }, ct);
}
