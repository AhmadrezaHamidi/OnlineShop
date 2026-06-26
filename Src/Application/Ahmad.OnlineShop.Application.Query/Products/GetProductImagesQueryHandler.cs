using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public class GetProductImagesQueryHandler : IQueryHandler<GetProductImagesQuery, List<ProductImageDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductImagesQueryHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<List<ProductImageDto>> HandleAsync(GetProductImagesQuery q, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(q.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        return product.Images
            .OrderBy(i => i.SortOrder)
            .Select(i => new ProductImageDto(i.Id, i.Url, i.Type, i.SortOrder, i.UploadedAt))
            .ToList();
    }
}
