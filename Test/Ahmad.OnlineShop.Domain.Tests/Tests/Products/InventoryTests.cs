/// <summary>
/// تست‌های Entity موجودی (Inventory)
/// پوشش‌دهنده: ایجاد، رزرو، آزادسازی، تأمین، تأیید، محاسبه موجودی موجود
/// خطاهای تست‌شده: InvalidQuantityException | InvalidReserveQuantityException
///                  InsufficientStockException | NothingToReleaseException
/// </summary>
namespace Ahmad.OnlineShop.Domain.Products.Tests;

public class InventoryTests
{
    private static Inventory CreateInventory(int initial = 100) =>
        Inventory.Create(1, 10, initial);

    // ── Create ───────────────────────────────────────────────────────────────

    /// <summary>ایجاد موجودی باید مقدار اولیه را ست و رزرو را صفر کند</summary>
    [Fact]
    public void Create_Should_Set_Initial_Quantity()
    {
        var inv = CreateInventory(50);

        Assert.Equal(50, inv.Quantity);
        Assert.Equal(0,  inv.ReservedQuantity);
        Assert.Equal(50, inv.AvailableQuantity);
    }

    /// <summary>خطا: مقدار اولیه منفی → InvalidQuantityException</summary>
    [Fact]
    public void Create_When_NegativeInitial_Should_Throw_InvalidQuantityException()
    {
        Assert.Throws<InvalidQuantityException>(() => Inventory.Create(1, 10, -1));
    }

    /// <summary>ایجاد با مقدار صفر باید موفق باشد</summary>
    [Fact]
    public void Create_With_Zero_Initial_Should_Succeed()
    {
        var inv = Inventory.Create(1, 10, 0);

        Assert.Equal(0, inv.AvailableQuantity);
    }

    // ── Reserve ──────────────────────────────────────────────────────────────

    /// <summary>رزرو باید مقدار رزرو را افزایش دهد</summary>
    [Fact]
    public void Reserve_Should_Increase_ReservedQuantity()
    {
        var inv = CreateInventory(100);
        inv.Reserve(30);

        Assert.Equal(30, inv.ReservedQuantity);
        Assert.Equal(70, inv.AvailableQuantity);
    }

    /// <summary>خطا: موجودی کافی نیست → InsufficientStockException</summary>
    [Fact]
    public void Reserve_When_InsufficientStock_Should_Throw_InsufficientStockException()
    {
        var inv = CreateInventory(10);

        Assert.Throws<InsufficientStockException>(() => inv.Reserve(20));
    }

    /// <summary>خطا: مقدار رزرو صفر → InvalidReserveQuantityException</summary>
    [Fact]
    public void Reserve_When_ZeroQuantity_Should_Throw_InvalidReserveQuantityException()
    {
        var inv = CreateInventory(100);

        Assert.Throws<InvalidReserveQuantityException>(() => inv.Reserve(0));
    }

    /// <summary>رزرو همه موجودی باید depleted=true برگرداند</summary>
    [Fact]
    public void Reserve_Returns_Depleted_True_When_NoStockLeft()
    {
        var inv           = CreateInventory(10);
        var (depleted, _) = inv.Reserve(10);

        Assert.True(depleted);
        Assert.Equal(0, inv.AvailableQuantity);
    }

    /// <summary>رزرو تا زیر آستانه کم‌موجودی باید lowStock=true برگرداند</summary>
    [Fact]
    public void Reserve_Returns_LowStock_True_When_Below_Threshold()
    {
        var inv           = CreateInventory(10);
        var (_, lowStock) = inv.Reserve(6);

        Assert.True(lowStock);
        Assert.Equal(4, inv.AvailableQuantity);
    }

    // ── Release ──────────────────────────────────────────────────────────────

    /// <summary>آزادسازی باید مقدار رزرو را کاهش دهد</summary>
    [Fact]
    public void Release_Should_Decrease_ReservedQuantity()
    {
        var inv = CreateInventory(100);
        inv.Reserve(50);
        inv.Release(20);

        Assert.Equal(30, inv.ReservedQuantity);
        Assert.Equal(70, inv.AvailableQuantity);
    }

    /// <summary>خطا: آزادسازی بیشتر از رزرو → NothingToReleaseException</summary>
    [Fact]
    public void Release_When_MoreThanReserved_Should_Throw_NothingToReleaseException()
    {
        var inv = CreateInventory(100);
        inv.Reserve(10);

        Assert.Throws<NothingToReleaseException>(() => inv.Release(20));
    }

    /// <summary>خطا: مقدار آزادسازی صفر → InvalidReserveQuantityException</summary>
    [Fact]
    public void Release_When_ZeroQuantity_Should_Throw_InvalidReserveQuantityException()
    {
        var inv = CreateInventory(100);

        Assert.Throws<InvalidReserveQuantityException>(() => inv.Release(0));
    }

    // ── Replenish ─────────────────────────────────────────────────────────────

    /// <summary>تأمین موجودی باید Quantity را افزایش دهد</summary>
    [Fact]
    public void Replenish_Should_Increase_Quantity()
    {
        var inv = CreateInventory(100);
        inv.Replenish(50);

        Assert.Equal(150, inv.Quantity);
        Assert.Equal(150, inv.AvailableQuantity);
    }

    /// <summary>خطا: مقدار تأمین صفر → InvalidQuantityException</summary>
    [Fact]
    public void Replenish_When_ZeroQuantity_Should_Throw_InvalidQuantityException()
    {
        var inv = CreateInventory(100);

        Assert.Throws<InvalidQuantityException>(() => inv.Replenish(0));
    }

    // ── Confirm ───────────────────────────────────────────────────────────────

    /// <summary>تأیید موجودی باید از هر دو رزرو و موجودی کل کسر کند</summary>
    [Fact]
    public void Confirm_Should_Deduct_From_Both_Reserved_And_Quantity()
    {
        var inv = CreateInventory(100);
        inv.Reserve(30);
        inv.Confirm(30);

        Assert.Equal(70, inv.Quantity);
        Assert.Equal(0,  inv.ReservedQuantity);
        Assert.Equal(70, inv.AvailableQuantity);
    }

    /// <summary>خطا: تأیید بیشتر از رزرو → NothingToReleaseException</summary>
    [Fact]
    public void Confirm_When_MoreThanReserved_Should_Throw_NothingToReleaseException()
    {
        var inv = CreateInventory(100);
        inv.Reserve(10);

        Assert.Throws<NothingToReleaseException>(() => inv.Confirm(20));
    }

    // ── IsLowStock / IsOutOfStock ─────────────────────────────────────────────

    /// <summary>IsLowStock باید True باشد وقتی موجودی ۵ یا کمتر است</summary>
    [Fact]
    public void IsLowStock_Should_Be_True_When_AvailableIs5OrLess()
    {
        var inv = CreateInventory(10);
        inv.Reserve(5);

        Assert.True(inv.IsLowStock);
    }

    /// <summary>IsOutOfStock باید True باشد وقتی هیچ موجودی‌ای نیست</summary>
    [Fact]
    public void IsOutOfStock_Should_Be_True_When_NoAvailableStock()
    {
        var inv = CreateInventory(5);
        inv.Reserve(5);

        Assert.True(inv.IsOutOfStock);
    }

    /// <summary>IsOutOfStock باید False باشد وقتی موجودی هست</summary>
    [Fact]
    public void IsOutOfStock_Should_Be_False_When_StockAvailable()
    {
        var inv = CreateInventory(10);

        Assert.False(inv.IsOutOfStock);
    }
}
