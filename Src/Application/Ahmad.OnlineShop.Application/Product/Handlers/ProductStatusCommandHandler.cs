using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public class ActivateProductCommandHandler : ICommandHandler<ActivateProductCommand, long>
{
    private readonly IProductRepository _productRepository;

    public ActivateProductCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(ActivateProductCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.Id, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.Activate();
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}

public class DeactivateProductCommandHandler : ICommandHandler<DeactivateProductCommand, long>
{
    private readonly IProductRepository _productRepository;

    public DeactivateProductCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(DeactivateProductCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.Id, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.Deactivate();
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}

public class ArchiveProductCommandHandler : ICommandHandler<ArchiveProductCommand, long>
{
    private readonly IProductRepository _productRepository;

    public ArchiveProductCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(ArchiveProductCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.Id, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.Archive();
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}
