using Ahmad.OnlineShop.Domain.Discount.Args;
using Ahmad.OnlineShop.Domain.Discount.Entities;
using Ahmad.OnlineShop.Domain.Discount.Events;
using Ahmad.OnlineShop.Domain.Discount.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Discount.Aggregates;

public sealed class ProductPackage : AggregateRoot<long>
{
    private readonly List<PackageItem> _items = [];

    public string   Title           { get; private set; } = string.Empty;
    public string?  Description     { get; private set; }
    public decimal  DiscountPercent { get; private set; }
    public DateTime ValidFrom       { get; private set; }
    public DateTime ValidTo         { get; private set; }
    public bool     IsActive        { get; private set; }
    public DateTime CreatedAt       { get; private set; }

    public IReadOnlyCollection<PackageItem> Items => _items.AsReadOnly();

    public bool IsValid => IsActive && DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidTo;

    private ProductPackage() { }

    private ProductPackage(CreatePackageArg arg) : base(arg.Id)
    {
        Title           = arg.Title.Trim();
        Description     = arg.Description;
        DiscountPercent = arg.DiscountPercent;
        ValidFrom       = arg.ValidFrom;
        ValidTo         = arg.ValidTo;
        IsActive        = false;
        CreatedAt       = DateTime.UtcNow;
    }

    public static ProductPackage Create(CreatePackageArg arg)
    {
        GuardTitle(arg.Title);
        GuardDiscountPercent(arg.DiscountPercent);
        GuardDateRange(arg.ValidFrom, arg.ValidTo);

        var pkg = new ProductPackage(arg);
        pkg.RaiseDomainEvent(new PackageCreatedEvent(arg.Id, arg.Title, arg.DiscountPercent));
        return pkg;
    }

    // ── Items ─────────────────────────────────────────────────────────────────

    public void AddItem(long itemId, long productId, int quantity)
    {
        if (_items.Any(i => i.ProductId == productId))
            throw new PackageItemAlreadyExistsException();

        _items.Add(new PackageItem(itemId, Id, productId, quantity));
    }

    public void RemoveItem(long productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) throw new PackageItemNotFoundException();
        _items.Remove(item);
    }

    // ── Status ────────────────────────────────────────────────────────────────

    public void Activate()
    {
        if (IsActive) throw new PackageAlreadyActiveException();
        if (DateTime.UtcNow > ValidTo) throw new PackageExpiredException();
        IsActive = true;
        RaiseDomainEvent(new PackageActivatedEvent(Id));
    }

    public void Deactivate()
    {
        if (!IsActive) throw new PackageNotActiveException();
        IsActive = false;
    }

    // ── Price Calculation ─────────────────────────────────────────────────────

    /// <summary>قیمت نهایی پکیج را با توجه به قیمت‌های محصولات محاسبه می‌کند.</summary>
    public decimal CalculateDiscountedTotal(decimal originalTotal)
        => Math.Round(originalTotal * (1 - DiscountPercent / 100), 0);

    // ── Guards ────────────────────────────────────────────────────────────────

    private static void GuardTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidPackageTitleException();
    }

    private static void GuardDiscountPercent(decimal percent)
    {
        if (percent is < 1 or > 100)
            throw new InvalidPackageDiscountException();
    }

    private static void GuardDateRange(DateTime from, DateTime to)
    {
        if (from >= to) throw new InvalidPackageDateRangeException();
    }
}
