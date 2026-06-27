using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Products.Args;

namespace Ahmad.OnlineShop.Application.Product.Mapper;

public static class ProductMapper
{
    public static CreateProductArg Map(this CreateProductCommand command, long id, long inventoryId)
        => new(
            Id:          id,
            SellerId:    command.SellerId,
            CategoryId:  command.CategoryId,
            Name:        command.Name,
            Description: command.Description,
            Price:       command.Price,
            InventoryId: inventoryId);

    public static CreateCategoryArg Map(this CreateCategoryCommand command, long id)
        => new(
            Id:       id,
            Name:     command.Name,
            ParentId: command.ParentId);

    public static CreateProductImageArg Map(this AddProductImageCommand command)
        => new(
            Id:        Guid.NewGuid(),
            ProductId: command.ProductId,
            Url:       command.Url,
            BucketKey: command.BucketKey,
            Type:      command.Type);
}
