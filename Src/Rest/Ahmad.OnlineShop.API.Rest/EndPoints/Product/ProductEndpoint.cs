using Ahmad.OnlineShop.Application.Contract.Products;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using AhmadBase.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ahmad.OnlineShop.Rest.EndPoints.Product;

public sealed class ProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Product");

        group.MapGetEndpoint(
            ProductConstants.Routes.GetProducts,
            Gets,
            ProductConstants.Names.GetProducts,
            ProductConstants.Docs.GetProducts.Summary,
            ProductConstants.Docs.GetProducts.Description);

        group.MapGetEndpoint(
            ProductConstants.Routes.GetProduct,
            Get,
            ProductConstants.Names.GetProduct,
            ProductConstants.Docs.GetProduct.Summary,
            ProductConstants.Docs.GetProduct.Description);

        group.MapPostEndpoint(           // ← Post
            ProductConstants.Routes.CreateProduct,
            PostProduct,
            ProductConstants.Names.CreateProduct,
            ProductConstants.Docs.CreateProduct.Summary,
            ProductConstants.Docs.CreateProduct.Description);

        group.MapPutEndpoint(            // ← Put
            ProductConstants.Routes.UpdateProduct,
            PutProduct,
            ProductConstants.Names.UpdateProduct,
            ProductConstants.Docs.UpdateProduct.Summary,
            ProductConstants.Docs.UpdateProduct.Description);

        group.MapPatchEndpoint(          // ← Patch
            ProductConstants.Routes.ChangePrice,
            PatchChangePrice,
            ProductConstants.Names.ChangePrice,
            ProductConstants.Docs.ChangePrice.Summary,
            ProductConstants.Docs.ChangePrice.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ActivateProduct,
            PatchActivateProduct,
            ProductConstants.Names.ActivateProduct,
            ProductConstants.Docs.ActivateProduct.Summary,
            ProductConstants.Docs.ActivateProduct.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.DeactivateProduct,
            PatchDeactivateProduct,
            ProductConstants.Names.DeactivateProduct,
            ProductConstants.Docs.DeactivateProduct.Summary,
            ProductConstants.Docs.DeactivateProduct.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ArchiveProduct,
            PatchArchiveProduct,
            ProductConstants.Names.ArchiveProduct,
            ProductConstants.Docs.ArchiveProduct.Summary,
            ProductConstants.Docs.ArchiveProduct.Description);
    }

    // ─── Handlers ─────────────────────────────────────────────────────────────
    private static async Task<GetProductsQueryResponse> Gets([AsParameters] GetProductsQuery query, IQueryBus queryBus, CancellationToken cancellation)
        => await queryBus.DispatchAsync<GetProductsQueryResponse>(query, cancellation);

    private static async Task<GetProductQueryResponse> Get(long id, IQueryBus queryBus, CancellationToken cancellation)
        => await queryBus.DispatchAsync<GetProductQueryResponse>(new GetProductQuery(id), cancellation);

    private static async Task<long> Post(CreateProductCommand command, ICommandBus sender, CancellationToken cancellation)
        => await sender.Dispatch<long>(command, cancellation);

    private static async Task<long> Put(long id, UpdateProductCommand command, ICommandBus sender, CancellationToken cancellation)
        => await sender.Dispatch<long>(command with { Id = id }, cancellation);

    private static async Task<long> PatchChangePrice(long id, ChangeProductPriceCommand command, ICommandBus sender, CancellationToken cancellation)
        => await sender.Dispatch<long>(command with { Id = id }, cancellation);

    private static async Task<long> PatchActivateProduct(long id, ICommandBus sender, CancellationToken cancellation)
        => await sender.Dispatch<long>(new ActivateProductCommand(id), cancellation);

    private static async Task<long> PatchDeactivateProduct(long id, ICommandBus sender, CancellationToken cancellation)
        => await sender.Dispatch<long>(new DeactivateProductCommand(id), cancellation);

    private static async Task<long> PatchArchiveProduct(long id, ICommandBus sender, CancellationToken cancellation)
        => await sender.Dispatch<long>(new ArchiveProductCommand(id), cancellation);
}