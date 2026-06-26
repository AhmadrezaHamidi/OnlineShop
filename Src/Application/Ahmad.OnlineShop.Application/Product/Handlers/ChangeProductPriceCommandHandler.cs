using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public class ChangeProductPriceCommandHandler : ICommandHandler<ChangeProductPriceCommand, long>
{
    private readonly IProductRepository _productRepository;

    public ChangeProductPriceCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(ChangeProductPriceCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.Id, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.ChangePrice(cmd.NewPrice);
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}
