using AhmadBase.Doamin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.Products.Exceptions;

// Product
public sealed class ProductNotFoundException : BusinessException
{
    public ProductNotFoundException()
        : base(Resources.Message.ProductNotFoundException) { }
}

public sealed class EmptyProductNameException : BusinessException
{
    public EmptyProductNameException()
        : base(Resources.Message.EmptyProductNameException) { }
}

public sealed class InvalidPriceException : BusinessException
{
    public InvalidPriceException()
        : base(Resources.Message.InvalidPriceException) { }
}

public sealed class ProductAlreadyArchivedException : BusinessException
{
    public ProductAlreadyArchivedException()
        : base(Resources.Message.ProductAlreadyArchivedException) { }
}

public sealed class ProductAlreadyActiveException : BusinessException
{
    public ProductAlreadyActiveException()
        : base(Resources.Message.ProductAlreadyActiveException) { }
}

public sealed class ProductAlreadyInactiveException : BusinessException
{
    public ProductAlreadyInactiveException()
        : base(Resources.Message.ProductAlreadyInactiveException) { }
}

// Category
public sealed class CategoryNotFoundException : BusinessException
{
    public CategoryNotFoundException()
        : base(Resources.Message.CategoryNotFoundException) { }
}

public sealed class EmptyCategoryNameException : BusinessException
{
    public EmptyCategoryNameException()
        : base(Resources.Message.EmptyCategoryNameException) { }
}

public sealed class CategoryCircularReferenceException : BusinessException
{
    public CategoryCircularReferenceException()
        : base(Resources.Message.CategoryCircularReferenceException) { }
}

// Inventory
public sealed class InsufficientStockException : BusinessException
{
    public InsufficientStockException()
        : base(Resources.Message.InsufficientStockException) { }
}

public sealed class InvalidQuantityException : BusinessException
{
    public InvalidQuantityException()
        : base(Resources.Message.InvalidQuantityException) { }
}

public sealed class InvalidReserveQuantityException : BusinessException
{
    public InvalidReserveQuantityException()
        : base(Resources.Message.InvalidReserveQuantityException) { }
}


public sealed class InvalidReserveException : BusinessException
{
    public InvalidReserveException()
        : base(Resources.Message.InvalidReserveException) { }
}

public sealed class NothingToReleaseException : BusinessException
{
    public NothingToReleaseException()
        : base(Resources.Message.NothingToReleaseException) { }
}

// Image
public sealed class ImageNotFoundException : BusinessException
{
    public ImageNotFoundException()
        : base(Resources.Message.ImageNotFoundException) { }
}

public sealed class InvalidImageUrlException : BusinessException
{
    public InvalidImageUrlException()
        : base(Resources.Message.InvalidImageUrlException) { }
}

public sealed class PrimaryImageExistsException : BusinessException
{
    public PrimaryImageExistsException()
        : base(Resources.Message.PrimaryImageExistsException) { }
}

public sealed class MaxImagesExceededException : BusinessException
{
    public MaxImagesExceededException()
        : base(Resources.Message.MaxImagesExceededException) { }
}


public sealed class ImageInvalidUrlException : BusinessException
{
    public ImageInvalidUrlException()
        : base(Resources.Message.ImageInvalidUrlException) { }
}
public sealed class ImageInvalidBucketKeyException : BusinessException
{
    public ImageInvalidBucketKeyException()
        : base(Resources.Message.ImageInvalidBucketKeyException) { }
}
public sealed class ImageInvalidSortOrderException : BusinessException
{
    public ImageInvalidSortOrderException()
        : base(Resources.Message.ImageInvalidSortOrderException) { }
}