using Ahmad.OnlineShop.Domain.Products.Enums;
using Ahmad.OnlineShop.Rest.EndPoints.Product;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Products");

        // â”€â”€ Queries â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        group.MapGetEndpoint<PagedResult<GetProductQueryResponse>>(
            ProductConstants.Routes.GetProducts,
            GetProducts,
            ProductConstants.Names.GetProducts,
            ProductConstants.Docs.GetProducts.Summary,
            ProductConstants.Docs.GetProducts.Description);

        group.MapGetEndpoint<GetProductQueryResponse>(
            ProductConstants.Routes.GetProduct,
            GetProduct,
            ProductConstants.Names.GetProduct,
            ProductConstants.Docs.GetProduct.Summary,
            ProductConstants.Docs.GetProduct.Description);

        // â”€â”€ Commands â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        group.MapPostEndpoint(
            ProductConstants.Routes.CreateProduct,
            CreateProduct,
            ProductConstants.Names.CreateProduct,
            ProductConstants.Docs.CreateProduct.Summary,
            ProductConstants.Docs.CreateProduct.Description);

        group.MapPutEndpoint(
            ProductConstants.Routes.UpdateProduct,
            UpdateProduct,
            ProductConstants.Names.UpdateProduct,
            ProductConstants.Docs.UpdateProduct.Summary,
            ProductConstants.Docs.UpdateProduct.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ChangePrice,
            ChangePrice,
            ProductConstants.Names.ChangePrice,
            ProductConstants.Docs.ChangePrice.Summary,
            ProductConstants.Docs.ChangePrice.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ActivateProduct,
            ActivateProduct,
            ProductConstants.Names.ActivateProduct,
            ProductConstants.Docs.ActivateProduct.Summary,
            ProductConstants.Docs.ActivateProduct.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.DeactivateProduct,
            DeactivateProduct,
            ProductConstants.Names.DeactivateProduct,
            ProductConstants.Docs.DeactivateProduct.Summary,
            ProductConstants.Docs.DeactivateProduct.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ArchiveProduct,
            ArchiveProduct,
            ProductConstants.Names.ArchiveProduct,
            ProductConstants.Docs.ArchiveProduct.Summary,
            ProductConstants.Docs.ArchiveProduct.Description);
    }

    // â”€â”€ Query Handlers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static async Task<PagedResult<GetProductQueryResponse>> GetProducts(
        IQueryBus      queryBus,
        CancellationToken ct,
        int            page       = 1,
        int            pageSize   = 20,
        string?        search     = null,
        long?          categoryId = null,
        ProductStatus? status     = null)
        => await queryBus.DispatchAsync<PagedResult<GetProductQueryResponse>>(
            new GetProductsQuery(page, pageSize, search, categoryId, status), ct);

    private static async Task<GetProductQueryResponse> GetProduct(
        long id,
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<GetProductQueryResponse>(new GetProductQuery(id), ct);

    // â”€â”€ Command Handlers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static async Task<long> CreateProduct(
        CreateProductCommand command,
        HttpContext          httpContext,
        ICommandBus          bus,
        CancellationToken    ct)
        => await bus.Dispatch<long>(command with { SellerId = httpContext.GetUserId() }, ct);

    private static async Task<long> UpdateProduct(
        long id,
        UpdateProductCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { Id = id }, ct);

    private static async Task<long> ChangePrice(
        long id,
        ChangeProductPriceCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { Id = id }, ct);

    private static async Task<long> ActivateProduct(
        long id,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(new ActivateProductCommand(id), ct);

    private static async Task<long> DeactivateProduct(
        long id,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(new DeactivateProductCommand(id), ct);

    private static async Task<long> ArchiveProduct(
        long id,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(new ArchiveProductCommand(id), ct);
}

