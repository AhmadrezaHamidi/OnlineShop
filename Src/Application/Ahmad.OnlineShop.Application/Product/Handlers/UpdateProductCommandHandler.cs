using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, long>
{
    private readonly IProductRepository  _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(
        IProductRepository  productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository  = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<long> Handle(UpdateProductCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.Id, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        var category = await _categoryRepository.GetByIdAsync(cmd.CategoryId, token);
        if (category is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.CategoryNotFound);
            throw new ProductDomainException(code, msg);
        }

        product.UpdateDetails(cmd.Name, cmd.Description, cmd.CategoryId);
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}
