using Ahmad.OnlineShop.Rest.EndPoints.Product;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public class CategoryEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Categories");

        group.MapGetEndpoint<List<GetCategoryQueryResponse>>(
            ProductConstants.Routes.GetCategories,
            GetCategories,
            ProductConstants.Names.GetCategories,
            ProductConstants.Docs.GetCategories.Summary,
            ProductConstants.Docs.GetCategories.Description);

        group.MapPostEndpoint(
            ProductConstants.Routes.CreateCategory,
            CreateCategory,
            ProductConstants.Names.CreateCategory,
            ProductConstants.Docs.CreateCategory.Summary,
            ProductConstants.Docs.CreateCategory.Description);

        group.MapPutEndpoint(
            ProductConstants.Routes.UpdateCategory,
            UpdateCategory,
            ProductConstants.Names.UpdateCategory,
            ProductConstants.Docs.UpdateCategory.Summary,
            ProductConstants.Docs.UpdateCategory.Description);
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<List<GetCategoryQueryResponse>> GetCategories(
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<List<GetCategoryQueryResponse>>(new GetCategoriesQuery(), ct);

    private static async Task<long> CreateCategory(
        CreateCategoryCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command, ct);

    private static async Task<long> UpdateCategory(
        long id,
        UpdateCategoryCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { Id = id }, ct);
}
