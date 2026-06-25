using Ahmad.OnlineShop.Application.Contract.Products;
using Ahmad.OnlineShop.Application.Product.Mapper;
using Ahmad.OnlineShop.Domain.Products;
using Ahmad.OnlineShop.Domain.Products.Args;
using Ahmad.OnlineShop.Domain.Products.Enums;
using Ahmad.OnlineShop.Domain.Products.Events;
using Ahmad.OnlineShop.Domain.Products.Exceptions;
using AhmadBase.Application;
using AhmadBase.Application.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Application.Product;

public class ProductHandlers(
    IProductRepository productRepository, IInventoryRepository inventoryRepository, IProductImageRepository imageRepository) :
    ICommandHandler<CreateProductCommand, long>,
    ICommandHandler<UpdateProductCommand, long>,
    ICommandHandler<ChangeProductPriceCommand, long>,
    ICommandHandler<ActivateProductCommand, long>,
    ICommandHandler<DeactivateProductCommand, long>,
    ICommandHandler<ArchiveProductCommand, long>,
    // Inventory Commands
    ICommandHandler<ReplenishStockCommand, long>,
    ICommandHandler<ReserveStockCommand, long>,
    ICommandHandler<ReleaseStockCommand, long>,
    ICommandHandler<ConfirmStockCommand, long>,
    // Image Commands
    ICommandHandler<AddProductImageCommand, Guid>,
    ICommandHandler<RemoveProductImageCommand, Guid>,
    ICommandHandler<SetPrimaryImageCommand, Guid>,
    ICommandHandler<ReorderImageCommand, Guid>
{
    #region Product Commands

    public async Task<long> Handle(CreateProductCommand command, CancellationToken token)
    {
        command.Id = productRepository.GetNextId();
        var arg = command.Map();
        var product = Domain.Products.Product.Create(arg);

        await productRepository.Add(product, token);
        return product.Id;
    }

    public async Task<long> Handle(UpdateProductCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.Id, token);
        if (product is null) throw new ProductNotFoundException();

        product.UpdateDetails(command.Name, command.Description, command.CategoryId);
        await productRepository.Update(product, token);
        return product.Id;
    }

    public async Task<long> Handle(ChangeProductPriceCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.Id, token);
        if (product is null) throw new ProductNotFoundException();

        product.ChangePrice(command.NewPrice);
        await productRepository.Update(product, token);
        return product.Id;
    }

    public async Task<long> Handle(ActivateProductCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.Id, token);
        if (product is null) throw new ProductNotFoundException();

        product.Activate();
        await productRepository.Update(product, token);
        return product.Id;
    }

    public async Task<long> Handle(DeactivateProductCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.Id, token);
        if (product is null) throw new ProductNotFoundException();

        product.Deactivate();
        await productRepository.Update(product, token);
        return product.Id;
    }

    public async Task<long> Handle(ArchiveProductCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.Id, token);
        if (product is null) throw new ProductNotFoundException();

        product.Archive();
        await productRepository.Update(product, token);
        return product.Id;
    }

    #endregion

    #region Inventory Commands

    public async Task<long> Handle(ReplenishStockCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        product.ReplenishStock(command.Quantity);
        await productRepository.Update(product, token);
        return product.Id;
    }

    public async Task<long> Handle(ReserveStockCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        product.ReserveStock(command.Quantity);
        await productRepository.Update(product, token);
        return product.Id;
    }

    public async Task<long> Handle(ReleaseStockCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        product.ReleaseStock(command.Quantity);
        await productRepository.Update(product, token);
        return product.Id;
    }

    public async Task<long> Handle(ConfirmStockCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        product.ConfirmStock(command.Quantity);
        await productRepository.Update(product, token);
        return product.Id;
    }

    #endregion

    #region Image Commands

    public async Task<Guid> Handle(AddProductImageCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        public ProductImage AddImage(CreateProductImageArg arg)
    {
        GuardMaxImagesNotExceeded();
        if (arg.Type == ImageType.Primary)
            GuardPrimaryImageNotExists();

        var image = ProductImage.Create(arg);
        _images.Add(image);

        RaiseDomainEvent(new ProductImageAddedEvent(Id, image.Id, arg.Url, arg.Type));
        return image;
    }
    var imageId = product.AddImage(command.ImageUrl, command.AltText, command.DisplayOrder);
        await productRepository.Update(product, token);
        return imageId;
    }

    public async Task<Guid> Handle(RemoveProductImageCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        product.RemoveImage(command.ImageId);
        await productRepository.Update(product, token);
        return command.ImageId;
    }

    public async Task<Guid> Handle(SetPrimaryImageCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        product.SetPrimaryImage(command.ImageId);
        await productRepository.Update(product, token);
        return command.ImageId;
    }

    public async Task<Guid> Handle(ReorderImageCommand command, CancellationToken token)
    {
        var product = await productRepository.Get(command.ProductId, token);
        if (product is null) throw new ProductNotFoundException();

        product.ReorderImage(command.ImageId, command.NewDisplayOrder);
        await productRepository.Update(product, token);
        return command.ImageId;
    }

    #endregion

    #region Queries

    public async Task<GetProductsQueryResponse> HandleAsync(GetProductsQuery query, CancellationToken token)
    {
        var (products, totalCount) = await productRepository.GetListAsync(
            // اینجا می‌تونی query.Page, query.PageSize, query.Search و ... رو پاس بدی
            token: token);

        var items = products.Select(ProductMapper.MapToDto).ToList();

        return new GetProductsQueryResponse(items, totalCount, 1, 10); // Page & PageSize رو بعداً کامل کن
    }

    public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery query, CancellationToken token)
    {
        var product = await productRepository.Get(query.Id, token);
        if (product is null) throw new ProductNotFoundException();

        return ProductMapper.MapToDetail(product);
    }

    public async Task<GetInventoryQueryResponse> HandleAsync(GetInventoryQuery query, CancellationToken token)
    {
        var inventory = await inventoryRepository.GetByProductId(query.ProductId, token);
        if (inventory is null) throw new ProductNotFoundException();

        return ProductMapper.MapInventory(inventory);
    }

    #endregion
}
