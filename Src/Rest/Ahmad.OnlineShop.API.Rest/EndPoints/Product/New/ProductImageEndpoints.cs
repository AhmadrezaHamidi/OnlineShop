using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public class ProductImageEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products/{productId:long}/images")
                       .WithTags("Product Images");

        // GET /api/products/{productId}/images
        group.MapGet("/", async (
            long              productId,
            IQueryBus         queryBus,
            CancellationToken token = default) =>
        {
            var result = await queryBus.Dispatch<List<ProductImageDto>>(
                new GetProductImagesQuery(productId), token);
            return Results.Ok(result);
        })
        .WithName("GetProductImages")
        .WithSummary("Get all images for a product")
        .Produces<List<ProductImageDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // POST /api/products/{productId}/images
        group.MapPost("/", async (
            long                   productId,
            AddProductImageRequest req,
            ICommandBus            bus,
            CancellationToken      token = default) =>
        {
            var cmd = new AddProductImageCommand(productId, req.Url, req.BucketKey, req.Type);
            await bus.Dispatch<long>(cmd, token);
            return Results.Created($"/api/products/{productId}/images", null);
        })
        .WithName("AddProductImage")
        .WithSummary("Add an image to a product")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // DELETE /api/products/{productId}/images/{imageId}
        group.MapDelete("/{imageId:guid}", async (
            long              productId,
            Guid              imageId,
            ICommandBus       bus,
            CancellationToken token = default) =>
        {
            await bus.Dispatch<Guid>(new RemoveProductImageCommand(productId, imageId), token);
            return Results.NoContent();
        })
        .WithName("RemoveProductImage")
        .WithSummary("Remove an image from a product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{productId}/images/{imageId}/primary
        group.MapPatch("/{imageId:guid}/primary", async (
            long              productId,
            Guid              imageId,
            ICommandBus       bus,
            CancellationToken token = default) =>
        {
            await bus.Dispatch<Guid>(new SetPrimaryImageCommand(productId, imageId), token);
            return Results.NoContent();
        })
        .WithName("SetPrimaryImage")
        .WithSummary("Set an image as the primary product image")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{productId}/images/{imageId}/reorder
        group.MapPatch("/{imageId:guid}/reorder", async (
            long                        productId,
            Guid                        imageId,
            ReorderProductImageRequest  req,
            ICommandBus                 bus,
            CancellationToken           token = default) =>
        {
            var cmd = new ReorderProductImageCommand(productId, imageId, req.NewSortOrder);
            await bus.Dispatch<Guid>(cmd, token);
            return Results.NoContent();
        })
        .WithName("ReorderProductImage")
        .WithSummary("Set the sort order of a product image")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }
}

// ── request body records ──────────────────────────────────────────────────────

public record AddProductImageRequest(string Url, string BucketKey, ImageType Type);
public record ReorderProductImageRequest(int NewSortOrder);
