using Ahmad.OnlineShop.Domain.Products.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Products;
public sealed class Inventory : TEntity<long>
{
    private const int LowStockThreshold = 5;

    public long ProductId { get; private set; }
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity => Quantity - ReservedQuantity;
    public DateTime UpdatedAt { get; private set; }
    public bool IsLowStock => AvailableQuantity <= LowStockThreshold;
    public bool IsOutOfStock => AvailableQuantity <= 0;

    private Inventory() { }

    private Inventory(long id, long productId, int initialQuantity)
    {
        Id = id;
        ProductId = productId;
        Quantity = initialQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Inventory Create(long id, long productId, int initialQuantity = 0)
    {
        GuardValidInitialQuantity(initialQuantity);
        return new Inventory(id, productId, initialQuantity);
    }

    public (bool depleted, bool lowStock) Reserve(int quantity)
    {
        GuardValidReserveQuantity(quantity);
        GuardSufficientStock(quantity);

        ReservedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;

        return (IsOutOfStock, IsLowStock);
    }

    public void Release(int quantity)
    {
        GuardValidReserveQuantity(quantity);
        GuardHasEnoughReservation(quantity);

        ReservedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Replenish(int quantity)
    {
        GuardValidReplenishQuantity(quantity);

        Quantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm(int quantity)
    {
        GuardHasEnoughReservation(quantity);

        ReservedQuantity -= quantity;
        Quantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    // ─── Guards ───────────────────────────────────────────

    private static void GuardValidInitialQuantity(int quantity)
    {
        if (quantity < 0)
            throw new InvalidQuantityException();
    }

    private static void GuardValidReserveQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidReserveQuantityException();
    }

    private static void GuardValidReplenishQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new InvalidQuantityException();
    }

    private void GuardSufficientStock(int quantity)
    {
        if (AvailableQuantity < quantity)
            throw new InsufficientStockException();
    }

    private void GuardHasEnoughReservation(int quantity)
    {
        if (ReservedQuantity < quantity)
            throw new NothingToReleaseException();
    }
}
