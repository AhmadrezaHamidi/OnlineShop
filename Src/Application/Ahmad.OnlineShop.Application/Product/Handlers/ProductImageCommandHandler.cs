using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public class AddProductImageCommandHandler : ICommandHandler<AddProductImageCommand, long>
{
    private readonly IProductRepository _productRepository;

    public AddProductImageCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<long> Handle(AddProductImageCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.AddImage(cmd.Url, cmd.BucketKey, cmd.Type);
        await _productRepository.UpdateAsync(product, token);
        return product.Id;
    }
}

public class RemoveProductImageCommandHandler : ICommandHandler<RemoveProductImageCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public RemoveProductImageCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<Guid> Handle(RemoveProductImageCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.RemoveImage(cmd.ImageId);
        await _productRepository.UpdateAsync(product, token);
        return cmd.ImageId;
    }
}

public class SetPrimaryImageCommandHandler : ICommandHandler<SetPrimaryImageCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public SetPrimaryImageCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<Guid> Handle(SetPrimaryImageCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        product.SetPrimaryImage(cmd.ImageId);
        await _productRepository.UpdateAsync(product, token);
        return cmd.ImageId;
    }
}

public class ReorderProductImageCommandHandler : ICommandHandler<ReorderProductImageCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public ReorderProductImageCommandHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<Guid> Handle(ReorderProductImageCommand cmd, CancellationToken token)
    {
        var product = await _productRepository.GetByIdAsync(cmd.ProductId, token);
        if (product is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.NotFound);
            throw new ProductDomainException(code, msg);
        }

        var image = product.Images.FirstOrDefault(i => i.Id == cmd.ImageId);
        if (image is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.ImageNotFound);
            throw new ProductDomainException(code, msg);
        }

        image.Reorder(cmd.NewSortOrder);
        await _productRepository.UpdateAsync(product, token);
        return cmd.ImageId;
    }
}
