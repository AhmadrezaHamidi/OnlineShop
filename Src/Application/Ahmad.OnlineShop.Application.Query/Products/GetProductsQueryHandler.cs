using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<PagedResult<ProductDto>> HandleAsync(GetProductsQuery q, CancellationToken token)
    {
        var (items, total) = await _productRepository.GetListAsync(
            q.Page,
            q.PageSize,
            q.Search,
            q.CategoryId,
            q.Status,
            token);

        var dtos = items.Select(product =>
        {
            var inventoryDto = new InventoryDto(
                product.Inventory.ProductId,
                product.Inventory.Quantity,
                product.Inventory.ReservedQuantity,
                product.Inventory.AvailableQuantity,
                product.Inventory.IsLowStock,
                product.Inventory.IsOutOfStock);

            var imageDtos = product.Images
                .Select(i => new ProductImageDto(i.Id, i.Url, i.Type, i.SortOrder, i.UploadedAt))
                .ToList();

            return new ProductDto(
                product.Id,
                product.CategoryId,
                product.Name,
                product.Description,
                product.Price,
                product.Status,
                product.CreatedAt,
                inventoryDto,
                imageDtos);
        }).ToList();

        return new PagedResult<ProductDto>(dtos, total, q.Page, q.PageSize);
    }
}
