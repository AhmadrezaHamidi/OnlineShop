using Ahmad.OnlineShop.Domain.Products.Enums;
using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Products
;

public sealed record ActivateProductCommand(long Id) : ICommand<long>;


public sealed record ArchiveProductCommand(long Id) : ICommand<long>;



public sealed record ReplenishStockCommand(
    long ProductId,
    int Quantity
) : ICommand<long>;

public sealed record ReserveStockCommand(
    long ProductId,
    int Quantity
) : ICommand<long>;

public sealed record ReleaseStockCommand(
    long ProductId,
    int Quantity
) : ICommand<long>;

public sealed record ConfirmStockCommand(
    long ProductId,
    int Quantity
) : ICommand<long>;
public sealed record AddProductImageCommand(
    long ProductId,
    string Url,
    string BucketKey,
    ImageType Type = ImageType.Gallery
) : ICommand<Guid>;

public sealed record RemoveProductImageCommand(
    long ProductId,
    Guid ImageId
) : ICommand<Guid>;

public sealed record SetPrimaryImageCommand(
    long ProductId,
    Guid ImageId
) : ICommand<Guid>;

public sealed record ReorderImageCommand(
    long ProductId,
    Guid ImageId,
    int NewSortOrder
) : ICommand<Guid>;
