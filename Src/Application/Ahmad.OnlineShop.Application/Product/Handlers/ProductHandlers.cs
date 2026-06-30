using Ahmad.OnlineShop.Application.Product.Mapper;
using Ahmad.OnlineShop.Domain.Products.Exceptions;
using Ahmad.OnlineShop.Persistence.EF;
using ProductAgg = Ahmad.OnlineShop.Domain.Products.Product;
using CategoryAgg = Ahmad.OnlineShop.Domain.Products.Category;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class ProductHandlers(
    IProductRepository  productRepository,
    ICategoryRepository categoryRepository,
    ApplicationDbContext context) :
    ICommandHandler<CreateProductCommand, long>,
    ICommandHandler<UpdateProductCommand, long>,
    ICommandHandler<ChangeProductPriceCommand, long>,
    ICommandHandler<ActivateProductCommand, long>,
    ICommandHandler<DeactivateProductCommand, long>,
    ICommandHandler<ArchiveProductCommand, long>,
    ICommandHandler<CreateCategoryCommand, long>,
    ICommandHandler<UpdateCategoryCommand, long>,
    ICommandHandler<ReserveStockCommand, long>,
    ICommandHandler<ReleaseStockCommand, long>,
    ICommandHandler<ConfirmStockCommand, long>,
    ICommandHandler<ReplenishStockCommand, long>,
    ICommandHandler<AddProductImageCommand, long>,
    ICommandHandler<RemoveProductImageCommand, Guid>,
    ICommandHandler<SetPrimaryImageCommand, Guid>,
    ICommandHandler<ReorderProductImageCommand, Guid>
{
    #region Product Commands

    public async Task<long> Handle(CreateProductCommand cmd, CancellationToken token)
    {
        _ = await categoryRepository.Get(cmd.CategoryId, token)
            ?? throw new CategoryNotFoundException();

        var productId = productRepository.GetNextId();
        var product = ProductAgg.Create(cmd.Map(productId, productId));

        await productRepository.AddAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<long> Handle(UpdateProductCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.Id, token)
            ?? throw new ProductNotFoundException();

        _ = await categoryRepository.Get(cmd.CategoryId, token)
            ?? throw new CategoryNotFoundException();

        product.UpdateDetails(cmd.Name, cmd.Description, cmd.CategoryId);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<long> Handle(ChangeProductPriceCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.Id, token)
            ?? throw new ProductNotFoundException();

        product.ChangePrice(cmd.NewPrice);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    #endregion

    #region Product Status Commands

    public async Task<long> Handle(ActivateProductCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.Id, token)
            ?? throw new ProductNotFoundException();

        product.Activate();
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<long> Handle(DeactivateProductCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.Id, token)
            ?? throw new ProductNotFoundException();

        product.Deactivate();
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<long> Handle(ArchiveProductCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.Id, token)
            ?? throw new ProductNotFoundException();

        product.Archive();
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    #endregion

    #region Category Commands

    public async Task<long> Handle(CreateCategoryCommand cmd, CancellationToken token)
    {
        if (await categoryRepository.ExistsByName(cmd.Name, token))
            throw new EmptyCategoryNameException();

        if (cmd.ParentId.HasValue)
            _ = await categoryRepository.Get(cmd.ParentId.Value, token)
                ?? throw new CategoryNotFoundException();

        var id = categoryRepository.GetNextId();
        var category = new CategoryAgg(id, cmd.Name, cmd.ParentId);

        await categoryRepository.Add(category, token);
        await context.SaveChangesAsync(token);
        return category.Id;
    }

    public async Task<long> Handle(UpdateCategoryCommand cmd, CancellationToken token)
    {
        var category = await categoryRepository.Get(cmd.Id, token)
            ?? throw new CategoryNotFoundException();

        if (cmd.ParentId.HasValue)
            _ = await categoryRepository.Get(cmd.ParentId.Value, token)
                ?? throw new CategoryNotFoundException();

        category.Rename(cmd.Name);
        category.ChangeParent(cmd.ParentId);

        await categoryRepository.Update(category, token);
        await context.SaveChangesAsync(token);
        return category.Id;
    }

    #endregion

    #region Stock Commands

    public async Task<long> Handle(ReserveStockCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        product.ReserveStock(cmd.Quantity);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<long> Handle(ReleaseStockCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        product.ReleaseStock(cmd.Quantity);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<long> Handle(ConfirmStockCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        product.ConfirmStock(cmd.Quantity);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<long> Handle(ReplenishStockCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        product.ReplenishStock(cmd.Quantity);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    #endregion

    #region Image Commands

    public async Task<long> Handle(AddProductImageCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        product.AddImage(cmd.Map());
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return product.Id;
    }

    public async Task<Guid> Handle(RemoveProductImageCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        product.RemoveImage(cmd.ImageId);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return cmd.ImageId;
    }

    public async Task<Guid> Handle(SetPrimaryImageCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        product.SetPrimaryImage(cmd.ImageId);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return cmd.ImageId;
    }

    public async Task<Guid> Handle(ReorderProductImageCommand cmd, CancellationToken token)
    {
        var product = await productRepository.Get(cmd.ProductId, token)
            ?? throw new ProductNotFoundException();

        var image = product.Images.FirstOrDefault(i => i.Id == cmd.ImageId)
            ?? throw new ImageNotFoundException();

        image.Reorder(cmd.NewSortOrder);
        await productRepository.UpdateAsync(product, token);
        await context.SaveChangesAsync(token);
        return cmd.ImageId;
    }

    #endregion
}
