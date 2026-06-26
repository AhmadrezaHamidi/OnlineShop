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

public class CategoryEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
                       .WithTags("Categories");

        // GET /api/categories
        group.MapGet("/", async (
            IQueryBus         queryBus,
            CancellationToken token = default) =>
        {
            var result = await queryBus.Dispatch<List<CategoryDto>>(
                new GetCategoriesQuery(), token);
            return Results.Ok(result);
        })
        .WithName("GetCategories")
        .WithSummary("Get all categories")
        .Produces<List<CategoryDto>>(StatusCodes.Status200OK)
        .WithOpenApi();

        // POST /api/categories
        group.MapPost("/", async (
            CreateCategoryCommand cmd,
            ICommandBus           bus,
            CancellationToken     token = default) =>
        {
            var id = await bus.Dispatch<long>(cmd, token);
            return Results.Created($"/api/categories/{id}", new { id });
        })
        .WithName("CreateCategory")
        .WithSummary("Create a new category")
        .Produces<object>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        // PUT /api/categories/{id}
        group.MapPut("/{id:long}", async (
            long                   id,
            UpdateCategoryRequest  req,
            ICommandBus            bus,
            CancellationToken      token = default) =>
        {
            var cmd = new UpdateCategoryCommand(id, req.Name, req.ParentId);
            await bus.Dispatch<long>(cmd, token);
            return Results.NoContent();
        })
        .WithName("UpdateCategory")
        .WithSummary("Update a category")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithOpenApi();
    }
}

// ── request body records ──────────────────────────────────────────────────────

public record UpdateCategoryRequest(string Name, long? ParentId);
