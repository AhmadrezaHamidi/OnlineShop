using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public class GetInventoryQueryHandler : IQueryHandler<GetInventoryQuery, InventoryDto>
{
    private readonly IProductRepository _productRepository;

    public GetInventoryQueryHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<InventoryDto> HandleAsync(GetInventoryQuery q, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(q.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        var inv = product.Inventory;
        return new InventoryDto(
            inv.ProductId,
            inv.Quantity,
            inv.ReservedQuantity,
            inv.AvailableQuantity,
            inv.IsLowStock,
            inv.IsOutOfStock);
    }
}
