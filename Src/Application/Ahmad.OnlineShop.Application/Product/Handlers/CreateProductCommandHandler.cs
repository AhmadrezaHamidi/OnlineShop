using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Aggregates;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, long>
{
    private readonly IProductRepository  _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(
        IProductRepository  productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository  = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<long> Handle(CreateProductCommand cmd, CancellationToken token)
    {
        var category = await _categoryRepository.GetByIdAsync(cmd.CategoryId, token);
        if (category is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.CategoryNotFound);
            throw new ProductDomainException(code, msg);
        }

        var productId   = await _productRepository.GetNextIdAsync();
        var inventoryId = productId; // 1-to-1: same numeric id for inventory

        var product = Product.Create(
            productId,
            cmd.CategoryId,
            cmd.Name,
            cmd.Description,
            cmd.Price,
            inventoryId);

        await _productRepository.AddAsync(product, token);
        return product.Id;
    }
}
