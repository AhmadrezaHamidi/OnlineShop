using Ahmad.OnlineShop.Domain.Products.Args;
using Ahmad.OnlineShop.Domain.Products.Enums;
using Ahmad.OnlineShop.Domain.Products.Events;
using Ahmad.OnlineShop.Domain.Products.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Products;

public class Product : AggregateRoot<long>
{
    private const int MaxImages = 10;

    private readonly List<ProductImage> _images    = [];
    private Inventory?                  _inventory;

    public long          SellerId    { get; private set; }
    public long          CategoryId  { get; private set; }
    public string        Name        { get; private set; } = string.Empty;
    public string?       Description { get; private set; }
    public decimal       Price       { get; private set; }
    public ProductStatus Status      { get; private set; }
    public DateTimeOffset CreationTime { get; private set; }

    public Inventory                     Inventory => _inventory!;
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    private Product() { }

    private Product(CreateProductArg arg) : base(arg.Id)
    {
        SellerId     = arg.SellerId;
        CategoryId   = arg.CategoryId;
        Name         = arg.Name.Trim();
        Description  = arg.Description;
        Price        = arg.Price;
        Status       = ProductStatus.Active;
        CreationTime = DateTimeOffset.UtcNow;
        _inventory   = Inventory.Create(arg.InventoryId, arg.Id, 0);
    }

    public static Product Create(CreateProductArg arg)
    {
        GuardName(arg.Name);
        GuardPrice(arg.Price);
        GuardSellerId(arg.SellerId);

        var product = new Product(arg);
        product.RaiseDomainEvent(new ProductCreatedEvent(arg.Id, arg.SellerId, arg.Name, arg.Price));
        return product;
    }

    // ── Ownership ─────────────────────────────────────────────────────

    /// <summary>بررسی می‌کند آیا فروشنده مالک این محصول است</summary>
    public bool BelongsToSeller(long sellerId) => SellerId == sellerId;

    /// <summary>اگر فروشنده مالک نباشد، خطا می‌دهد</summary>
    public void GuardSellerOwnership(long sellerId)
    {
        if (!BelongsToSeller(sellerId))
            throw new ProductNotOwnedBySellerException();
    }

    // ── Details ────────────────────────────────────────────────────────

    public void UpdateDetails(string name, string? description, long categoryId)
    {
        GuardName(name);
        Name        = name.Trim();
        Description = description;
        CategoryId  = categoryId;
    }

    public void ChangePrice(decimal newPrice)
    {
        GuardPrice(newPrice);
        var old = Price;
        Price   = newPrice;
        RaiseDomainEvent(new ProductPriceChangedEvent(Id, old, newPrice));
    }

    // ── Status ─────────────────────────────────────────────────────────

    public void Activate()
    {
        GuardNotAlreadyActive();
        Status = ProductStatus.Active;
        RaiseDomainEvent(new ProductStatusChangedEvent(Id, Status));
    }

    public void Deactivate()
    {
        GuardNotAlreadyInactive();
        Status = ProductStatus.Inactive;
        RaiseDomainEvent(new ProductStatusChangedEvent(Id, Status));
    }

    public void Archive()
    {
        GuardNotAlreadyArchived();
        Status = ProductStatus.Archived;
        RaiseDomainEvent(new ProductStatusChangedEvent(Id, Status));
    }

    // ── Inventory ──────────────────────────────────────────────────────

    public void ReserveStock(int quantity)
    {
        var (depleted, lowStock) = _inventory!.Reserve(quantity);
        RaiseDomainEvent(new StockReservedEvent(Id, quantity, _inventory.AvailableQuantity));

        if (depleted)
            RaiseDomainEvent(new StockDepletedEvent(Id, _inventory.AvailableQuantity));
        else if (lowStock)
            RaiseDomainEvent(new StockDepletedEvent(Id, _inventory.AvailableQuantity));
    }

    public void ReleaseStock(int quantity)
    {
        _inventory!.Release(quantity);
        RaiseDomainEvent(new StockReleasedEvent(Id, quantity));
    }

    public void ConfirmStock(int quantity)  => _inventory!.Confirm(quantity);

    public void ReplenishStock(int quantity)
    {
        _inventory!.Replenish(quantity);
        RaiseDomainEvent(new StockReplenishedEvent(Id, quantity, _inventory.Quantity));
    }

    // ── Images ─────────────────────────────────────────────────────────

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

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        GuardImageExists(image);
        _images.Remove(image!);
        RaiseDomainEvent(new ProductImageRemovedEvent(Id, imageId));
    }

    public void SetPrimaryImage(Guid imageId)
    {
        var target = _images.FirstOrDefault(i => i.Id == imageId);
        GuardImageExists(target);

        foreach (var img in _images.Where(i => i.Type == ImageType.Primary))
            img.ChangeType(ImageType.Gallery);

        target!.ChangeType(ImageType.Primary);
    }

    // ── Guards ─────────────────────────────────────────────────────────

    private static void GuardName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EmptyProductNameException();
    }

    private static void GuardPrice(decimal price)
    {
        if (price <= 0)
            throw new InvalidPriceException();
    }

    private static void GuardSellerId(long sellerId)
    {
        if (sellerId <= 0)
            throw new InvalidSellerIdException();
    }

    private void GuardNotAlreadyActive()
    {
        if (Status == ProductStatus.Active)
            throw new ProductAlreadyActiveException();   // ← fix: قبلاً اشتباه Archived بود
    }

    private void GuardNotAlreadyInactive()
    {
        if (Status == ProductStatus.Inactive)
            throw new ProductAlreadyInactiveException();
    }

    private void GuardNotAlreadyArchived()
    {
        if (Status == ProductStatus.Archived)
            throw new ProductAlreadyArchivedException();
    }

    private void GuardMaxImagesNotExceeded()
    {
        if (_images.Count >= MaxImages)
            throw new MaxImagesExceededException();
    }

    private void GuardPrimaryImageNotExists()
    {
        if (_images.Any(i => i.Type == ImageType.Primary))
            throw new PrimaryImageExistsException();
    }

    private void GuardImageExists(ProductImage? image)
    {
        if (image is null)
            throw new ImageNotFoundException();
    }
}
