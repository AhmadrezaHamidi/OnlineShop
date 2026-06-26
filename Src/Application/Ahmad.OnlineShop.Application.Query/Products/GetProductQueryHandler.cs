using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<ProductDto> HandleAsync(GetProductQuery q, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(q.Id, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

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
    }
}
