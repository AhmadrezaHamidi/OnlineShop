using Ahmad.OnlineShop.Domain.Products.Exceptions;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class ProductQueryHandlers(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository) :
    IQueryHandler<GetProductQuery, GetProductQueryResponse>,
    IQueryHandler<GetProductsQuery, QueryPagedResult<GetProductQueryResponse>>,
    IQueryHandler<GetProductImagesQuery, List<GetProductImageResponse>>,
    IQueryHandler<GetInventoryQuery, GetProductInventoryResponse>,
    IQueryHandler<GetCategoriesQuery, List<GetCategoryQueryResponse>>
{
    public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery q, CancellationToken token)
    {
        var product = await productRepository.Get(q.Id, token)
            ?? throw new ProductNotFoundException();

        return product.ToResponse();
    }

    public async Task<QueryPagedResult<GetProductQueryResponse>> HandleAsync(GetProductsQuery q, CancellationToken token)
    {
        var (items, total) = await productRepository.GetListAsync(
            q.Page, q.PageSize, q.Search, q.CategoryId, q.Status, token);

        return new QueryPagedResult<GetProductQueryResponse>(
            items.Select(p => p.ToResponse()).ToList(),
            total, q.Page, q.PageSize);
    }

    public async Task<List<GetProductImageResponse>> HandleAsync(GetProductImagesQuery q, CancellationToken token)
    {
        var product = await productRepository.Get(q.ProductId, token)
            ?? throw new ProductNotFoundException();

        return product.Images
            .OrderBy(i => i.SortOrder)
            .Select(i => i.ToResponse())
            .ToList();
    }

    public async Task<GetProductInventoryResponse> HandleAsync(GetInventoryQuery q, CancellationToken token)
    {
        var product = await productRepository.Get(q.ProductId, token)
            ?? throw new ProductNotFoundException();

        return product.Inventory.ToResponse();
    }

    public async Task<List<GetCategoryQueryResponse>> HandleAsync(GetCategoriesQuery q, CancellationToken token)
    {
        var categories = await categoryRepository.Gets(token);
        return categories.Select(c => c.ToResponse()).ToList();
    }
}
