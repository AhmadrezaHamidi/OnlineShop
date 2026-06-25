using Ahmad.OnlineShop.Application.Contract.Products;
using Ahmad.OnlineShop.Domain.Products;
using Ahmad.OnlineShop.Domain.Products.Args;
using System.Linq;

namespace Ahmad.OnlineShop.Application.Product.Mapper;



public static class ProductMapper
{
    #region Product Mappers


    public static CreateProductArg Map(this CreateProductCommand command)
    {
        return new CreateProductArg(
            Id: command.Id,
            CategoryId: command.CategoryId,
            Name: command.Name,
            Description: command.Description,
            Price: command.Price,
            InventoryId: command.InventoryId
        );
    }


    //public static ProductDto MapToDto(Product product)
    //{
    //    return new ProductDto(
    //        Id: product.Id,
    //        Name: product.Name,
    //        Price: product.Price,
    //        SKU: product.SKU,           // اگر SKU در Domain داری
    //        IsActive: product.Status == ProductStatus.Active
    //    );
    //}

    //public static GetProductQueryResponse MapToDetail(Product product)
    //{
    //    return new GetProductQueryResponse(
    //        Id: product.Id,
    //        Name: product.Name,
    //        Description: product.Description,
    //        Price: product.Price,
    //        SKU: product.SKU,
    //        CategoryId: product.CategoryId,
    //        IsActive: product.Status == ProductStatus.Active,
    //        IsArchived: product.Status == ProductStatus.Archived,
    //        Images: product.Images.Select(MapImage).ToList(),
    //        Inventory: MapInventory(product.Inventory)
    //    );
    //}

    //#endregion

    //#region Inventory Mappers

    //public static GetInventoryQueryResponse MapInventory(Inventory inventory)
    //{
    //    return new GetInventoryQueryResponse(
    //        ProductId: inventory.ProductId,
    //        CurrentStock: inventory.Quantity,
    //        ReservedStock: inventory.ReservedQuantity,
    //        AvailableStock: inventory.AvailableQuantity
    //    );
    //}

    //#endregion

    //#region Image Mappers

    //public static ProductImageDto MapImage(ProductImage image)
    //{
    //    return new ProductImageDto(
    //        Id: image.Id,
    //        ImageUrl: image.Url,
    //        AltText: image.AltText ?? string.Empty,     // اگر AltText داری
    //        DisplayOrder: image.SortOrder,
    //        IsPrimary: image.Type == ImageType.Primary
    //    );
    //}

    //public static List<ProductImageDto> MapImages(IEnumerable<ProductImage> images)
    //{
    //    return images
    //        .OrderBy(i => i.SortOrder)
    //        .Select(MapImage)
    //        .ToList();
    //}

    //#endregion

    //#region List Mappers

    //public static GetProductsQueryResponse MapToListResponse(
    //    List<Product> products,
    //    int totalCount,
    //    int page = 1,
    //    int pageSize = 20)
    //{
    //    var items = products.Select(MapToDto).ToList();

    //    return new GetProductsQueryResponse(
    //        Items: items,
    //        TotalCount: totalCount,
    //        Page: page,
    //        PageSize: pageSize
    //    );
    //}

    #endregion
}