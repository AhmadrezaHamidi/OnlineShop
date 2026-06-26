using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public class ReserveStockCommandHandler : ICommandHandler<ReserveStockCommand, long>
{
    private readonly IProductRepository _productRepository;

    public ReserveStockCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(ReserveStockCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.ReserveStock(cmd.Quantity);
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}

public class ReleaseStockCommandHandler : ICommandHandler<ReleaseStockCommand, long>
{
    private readonly IProductRepository _productRepository;

    public ReleaseStockCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(ReleaseStockCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.ReleaseStock(cmd.Quantity);
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}

public class ConfirmStockCommandHandler : ICommandHandler<ConfirmStockCommand, long>
{
    private readonly IProductRepository _productRepository;

    public ConfirmStockCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(ConfirmStockCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.ConfirmStock(cmd.Quantity);
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}

public class ReplenishStockCommandHandler : ICommandHandler<ReplenishStockCommand, long>
{
    private readonly IProductRepository _productRepository;

    public ReplenishStockCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(ReplenishStockCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.ReplenishStock(cmd.Quantity);
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}
