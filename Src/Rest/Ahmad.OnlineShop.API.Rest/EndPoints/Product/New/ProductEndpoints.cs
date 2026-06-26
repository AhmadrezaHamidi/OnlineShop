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

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
                       .WithTags("Products");

        // GET /api/products
        group.MapGet("/", async (
            IQueryBus    queryBus,
            int          page       = 1,
            int          pageSize   = 20,
            string?      search     = null,
            long?        categoryId = null,
            ProductStatus? status   = null,
            CancellationToken token = default) =>
        {
            var query  = new GetProductsQuery(page, pageSize, search, categoryId, status);
            var result = await queryBus.Dispatch<PagedResult<ProductDto>>(query, token);
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithSummary("Get paged list of products")
        .Produces<PagedResult<ProductDto>>(StatusCodes.Status200OK)
        .WithOpenApi();

        // GET /api/products/{id}
        group.MapGet("/{id:long}", async (
            long              id,
            IQueryBus         queryBus,
            CancellationToken token = default) =>
        {
            var result = await queryBus.Dispatch<ProductDto>(new GetProductQuery(id), token);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetProductById")
        .WithSummary("Get product by id")
        .Produces<ProductDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // POST /api/products
        group.MapPost("/", async (
            CreateProductCommand cmd,
            ICommandBus          bus,
            CancellationToken    token = default) =>
        {
            var id = await bus.Dispatch<long>(cmd, token);
            return Results.Created($"/api/products/{id}", new { id });
        })
        .WithName("CreateProduct")
        .WithSummary("Create a new product")
        .Produces<object>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        // PUT /api/products/{id}
        group.MapPut("/{id:long}", async (
            long                 id,
            UpdateProductRequest req,
            ICommandBus          bus,
            CancellationToken    token = default) =>
        {
            var cmd = new UpdateProductCommand(id, req.Name, req.Description, req.CategoryId);
            await bus.Dispatch<long>(cmd, token);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .WithSummary("Update product details")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{id}/price
        group.MapPatch("/{id:long}/price", async (
            long                     id,
            ChangePriceRequest       req,
            ICommandBus              bus,
            CancellationToken        token = default) =>
        {
            var cmd = new ChangeProductPriceCommand(id, req.NewPrice);
            await bus.Dispatch<long>(cmd, token);
            return Results.NoContent();
        })
        .WithName("ChangeProductPrice")
        .WithSummary("Change product price")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{id}/activate
        group.MapPatch("/{id:long}/activate", async (
            long              id,
            ICommandBus       bus,
            CancellationToken token = default) =>
        {
            await bus.Dispatch<long>(new ActivateProductCommand(id), token);
            return Results.NoContent();
        })
        .WithName("ActivateProduct")
        .WithSummary("Activate a product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{id}/deactivate
        group.MapPatch("/{id:long}/deactivate", async (
            long              id,
            ICommandBus       bus,
            CancellationToken token = default) =>
        {
            await bus.Dispatch<long>(new DeactivateProductCommand(id), token);
            return Results.NoContent();
        })
        .WithName("DeactivateProduct")
        .WithSummary("Deactivate a product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();

        // PATCH /api/products/{id}/archive
        group.MapPatch("/{id:long}/archive", async (
            long              id,
            ICommandBus       bus,
            CancellationToken token = default) =>
        {
            await bus.Dispatch<long>(new ArchiveProductCommand(id), token);
            return Results.NoContent();
        })
        .WithName("ArchiveProduct")
        .WithSummary("Archive a product")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }
}

// ── request body records ──────────────────────────────────────────────────────

public record UpdateProductRequest(string Name, string? Description, long CategoryId);
public record ChangePriceRequest(decimal NewPrice);
