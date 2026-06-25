using AhmadBase.Application;
using AhmadBase.Web;
using Ahmad.OnlineShop.Application.Contract.Product;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.EndPoints.Product;

public sealed class ProductImagesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Product.Images");

        group.MapPostEndpoint(
            ProductConstants.Routes.AddImage,
            PostImage,
            ProductConstants.Names.AddImage,
            ProductConstants.Docs.AddImage.Summary,
            ProductConstants.Docs.AddImage.Description);

        group.MapDeleteEndpoint(
            ProductConstants.Routes.RemoveImage,
            DeleteImage,
            ProductConstants.Names.RemoveImage,
            ProductConstants.Docs.RemoveImage.Summary,
            ProductConstants.Docs.RemoveImage.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.SetPrimaryImage,
            PatchSetPrimaryImage,
            ProductConstants.Names.SetPrimaryImage,
            ProductConstants.Docs.SetPrimaryImage.Summary,
            ProductConstants.Docs.SetPrimaryImage.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReorderImage,
            PatchReorderImage,
            ProductConstants.Names.ReorderImage,
            ProductConstants.Docs.ReorderImage.Summary,
            ProductConstants.Docs.ReorderImage.Description);
    }

    // ─── Handlers ─────────────────────────────────────────────────────────────
    private static async Task<Guid> PostImage(
        long id,
        AddProductImageCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<Guid>(command with { ProductId = id }, cancellation);

    private static async Task<Guid> DeleteImage(
        long id,
        Guid imageId,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<Guid>(new RemoveProductImageCommand(id, imageId), cancellation);

    private static async Task<Guid> PatchSetPrimaryImage(
        long id,
        Guid imageId,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<Guid>(new SetPrimaryImageCommand(id, imageId), cancellation);

    private static async Task<Guid> PatchReorderImage(
        long id,
        Guid imageId,
        ReorderImageCommand command,
        ICommandBus sender,
        CancellationToken cancellation)
        => await sender.Dispatch<Guid>(command with { ProductId = id, ImageId = imageId }, cancellation);
}