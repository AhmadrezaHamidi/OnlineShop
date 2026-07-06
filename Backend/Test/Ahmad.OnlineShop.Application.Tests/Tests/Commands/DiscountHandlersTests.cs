/// <summary>
/// تست‌های Application Handler تخفیف (DiscountHandlers)
/// پوشش‌دهنده: ایجاد/فعال‌سازی/غیرفعال‌سازی/اعمال کد تخفیف + مدیریت پکیج
/// تکنولوژی: Fake Repository
/// خطاهای تست‌شده: DiscountNotFoundException | DiscountCodeAlreadyExistsException |
///                  DiscountNotActiveException | ProductPackageNotFoundException |
///                  PackageItemAlreadyExistsException | PackageItemNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Handlers.Discount;
using DiscountAggregate = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;
using PackageAgg        = Ahmad.OnlineShop.Domain.Discount.Aggregates.ProductPackage;

namespace Ahmad.OnlineShop.Application.Tests.Commands;

public class DiscountHandlersTests
{
    private readonly FakeDiscountRepository       _discountRepo = new();
    private readonly FakeProductPackageRepository _packageRepo  = new();
    private readonly DiscountHandlers             _sut;
    private readonly CancellationToken            _ct = CancellationToken.None;

    public DiscountHandlersTests()
    {
        _sut = new DiscountHandlers(_discountRepo, _packageRepo, FakeAppDb.Create());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

    private static DiscountAggregate MakePercentDiscount(long id = 1, string code = "SAVE10") =>
        DiscountAggregate.Create(new CreateDiscountArg(
            id, code, "تخفیف ۱۰٪",
            DiscountType.Percentage, 10m,
            MinOrderAmount: null, MaxUsage: null, ExpiresAt: null));

    private static DiscountAggregate MakeFixedDiscount(long id = 2, string code = "FIXED50K") =>
        DiscountAggregate.Create(new CreateDiscountArg(
            id, code, "تخفیف ۵۰ هزار تومان",
            DiscountType.FixedAmount, 50_000m,
            MinOrderAmount: 200_000m, MaxUsage: 5, ExpiresAt: null));

    private static PackageAgg MakePackage(long id = 1) =>
        PackageAgg.Create(new CreatePackageArg(
            id, "پکیج تابستانه", "توضیحات",
            DiscountPercent: 15m,
            ValidFrom: DateTime.UtcNow.AddDays(-1),
            ValidTo: DateTime.UtcNow.AddDays(30)));

    // ── CreateDiscountCommand ─────────────────────────────────────────────────────

    /// <summary>ایجاد کد تخفیف جدید باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task CreateDiscount_When_CodeNotExists_Should_AddDiscountAndReturnId()
    {
        _discountRepo.CodeExists = false;

        var id = await _sut.Handle(
            new CreateDiscountCommand("SAVE10", "تخفیف ۱۰٪", DiscountType.Percentage, 10m, null, null, null), _ct);

        Assert.NotNull(_discountRepo.Added);
        Assert.Equal("SAVE10", _discountRepo.Added!.Code);
        Assert.True(id > 0);
    }

    /// <summary>خطا: کد تکراری → DiscountCodeAlreadyExistsException</summary>
    [Fact]
    public async Task CreateDiscount_When_CodeExists_Should_Throw_DiscountCodeAlreadyExistsException()
    {
        _discountRepo.CodeExists = true;

        await Assert.ThrowsAsync<DiscountCodeAlreadyExistsException>(
            () => _sut.Handle(
                new CreateDiscountCommand("SAVE10", "تخفیف", DiscountType.Percentage, 10m, null, null, null), _ct));

        Assert.Null(_discountRepo.Added);
    }

    // ── ActivateDiscountCommand ───────────────────────────────────────────────────

    /// <summary>فعال‌سازی تخفیف غیرفعال باید IsActive را true کند</summary>
    [Fact]
    public async Task ActivateDiscount_When_DiscountExists_And_Inactive_Should_Activate()
    {
        var discount = MakePercentDiscount();
        discount.Deactivate();
        _discountRepo.Seed(discount);

        var result = await _sut.Handle(new ActivateDiscountCommand(1), _ct);

        Assert.True(result);
        Assert.True(discount.IsActive);
        Assert.NotNull(_discountRepo.Updated);
    }

    /// <summary>خطا: تخفیف پیدا نشد → DiscountNotFoundException</summary>
    [Fact]
    public async Task ActivateDiscount_When_NotFound_Should_Throw_DiscountNotFoundException()
    {
        await Assert.ThrowsAsync<DiscountNotFoundException>(
            () => _sut.Handle(new ActivateDiscountCommand(99), _ct));
    }

    // ── DeactivateDiscountCommand ─────────────────────────────────────────────────

    /// <summary>غیرفعال‌سازی تخفیف فعال باید IsActive را false کند</summary>
    [Fact]
    public async Task DeactivateDiscount_When_DiscountExists_And_Active_Should_Deactivate()
    {
        var discount = MakePercentDiscount();
        _discountRepo.Seed(discount);

        var result = await _sut.Handle(new DeactivateDiscountCommand(1), _ct);

        Assert.True(result);
        Assert.False(discount.IsActive);
        Assert.NotNull(_discountRepo.Updated);
    }

    /// <summary>خطا: تخفیف پیدا نشد → DiscountNotFoundException</summary>
    [Fact]
    public async Task DeactivateDiscount_When_NotFound_Should_Throw_DiscountNotFoundException()
    {
        await Assert.ThrowsAsync<DiscountNotFoundException>(
            () => _sut.Handle(new DeactivateDiscountCommand(99), _ct));
    }

    // ── ApplyDiscountCommand ──────────────────────────────────────────────────────

    /// <summary>اعمال تخفیف درصدی باید مبلغ درست را برگرداند</summary>
    [Fact]
    public async Task ApplyDiscount_Percentage_Should_ReturnCorrectAmount()
    {
        var discount = MakePercentDiscount(code: "SAVE10");
        _discountRepo.Seed(discount);

        var amount = await _sut.Handle(new ApplyDiscountCommand("SAVE10", 1_000_000m), _ct);

        Assert.Equal(100_000m, amount);
        Assert.Equal(1, discount.UsageCount);
    }

    /// <summary>اعمال تخفیف مبلغ ثابت با رعایت حداقل سفارش</summary>
    [Fact]
    public async Task ApplyDiscount_FixedAmount_With_MinOrderMet_Should_ReturnFixedValue()
    {
        var discount = MakeFixedDiscount(code: "FIXED50K");
        _discountRepo.Seed(discount);

        var amount = await _sut.Handle(new ApplyDiscountCommand("FIXED50K", 500_000m), _ct);

        Assert.Equal(50_000m, amount);
    }

    /// <summary>خطا: اعمال تخفیف غیرفعال → DiscountNotActiveException</summary>
    [Fact]
    public async Task ApplyDiscount_When_Inactive_Should_Throw_DiscountNotActiveException()
    {
        var discount = MakePercentDiscount(code: "SAVE10");
        discount.Deactivate();
        _discountRepo.Seed(discount);

        await Assert.ThrowsAsync<DiscountNotActiveException>(
            () => _sut.Handle(new ApplyDiscountCommand("SAVE10", 1_000_000m), _ct));
    }

    /// <summary>خطا: حداقل مبلغ سفارش برآورده نشده → DiscountMinOrderAmountNotMetException</summary>
    [Fact]
    public async Task ApplyDiscount_When_OrderAmountBelowMinimum_Should_Throw()
    {
        var discount = MakeFixedDiscount(code: "FIXED50K");
        _discountRepo.Seed(discount);

        await Assert.ThrowsAsync<DiscountMinOrderAmountNotMetException>(
            () => _sut.Handle(new ApplyDiscountCommand("FIXED50K", 100_000m), _ct));
    }

    /// <summary>خطا: حداکثر استفاده → DiscountMaxUsageReachedException</summary>
    [Fact]
    public async Task ApplyDiscount_When_MaxUsageReached_Should_Throw()
    {
        var arg = new CreateDiscountArg(3, "MAXTEST", "کد محدود",
            DiscountType.Percentage, 5m,
            MinOrderAmount: null, MaxUsage: 2, ExpiresAt: null);
        var discount = DiscountAggregate.Create(arg);
        discount.Apply(100_000m);
        discount.Apply(100_000m);
        _discountRepo.Seed(discount);

        await Assert.ThrowsAsync<DiscountMaxUsageReachedException>(
            () => _sut.Handle(new ApplyDiscountCommand("MAXTEST", 100_000m), _ct));
    }

    /// <summary>خطا: کد تخفیف پیدا نشد → DiscountNotFoundException</summary>
    [Fact]
    public async Task ApplyDiscount_When_CodeNotFound_Should_Throw_DiscountNotFoundException()
    {
        await Assert.ThrowsAsync<DiscountNotFoundException>(
            () => _sut.Handle(new ApplyDiscountCommand("NOTEXIST", 1_000_000m), _ct));
    }

    // ── CreatePackageCommand ──────────────────────────────────────────────────────

    /// <summary>ایجاد پکیج جدید باید آن را ذخیره کند و شناسه برگرداند</summary>
    [Fact]
    public async Task CreatePackage_Should_AddPackageAndReturnId()
    {
        var id = await _sut.Handle(
            new CreatePackageCommand(
                "پکیج بهاره", "توضیح",
                DiscountPercent: 20m,
                ValidFrom: DateTime.UtcNow.AddDays(1),
                ValidTo: DateTime.UtcNow.AddDays(30)), _ct);

        Assert.NotNull(_packageRepo.Added);
        Assert.Equal("پکیج بهاره", _packageRepo.Added!.Title);
        Assert.True(id > 0);
    }

    // ── AddPackageItemCommand ─────────────────────────────────────────────────────

    /// <summary>افزودن محصول به پکیج باید آیتم را اضافه کند</summary>
    [Fact]
    public async Task AddPackageItem_When_PackageExists_Should_AddItem()
    {
        var pkg = MakePackage();
        _packageRepo.Seed(pkg);

        var result = await _sut.Handle(new AddPackageItemCommand(1, 101, 2), _ct);

        Assert.True(result);
        Assert.Single(pkg.Items);
        Assert.Equal(101L, pkg.Items.First().ProductId);
    }

    /// <summary>خطا: محصول تکراری در پکیج → PackageItemAlreadyExistsException</summary>
    [Fact]
    public async Task AddPackageItem_When_ProductAlreadyInPackage_Should_Throw()
    {
        var pkg = MakePackage();
        pkg.AddItem(10, 101, 1);
        _packageRepo.Seed(pkg);

        await Assert.ThrowsAsync<PackageItemAlreadyExistsException>(
            () => _sut.Handle(new AddPackageItemCommand(1, 101, 1), _ct));
    }

    /// <summary>خطا: پکیج پیدا نشد → ProductPackageNotFoundException</summary>
    [Fact]
    public async Task AddPackageItem_When_PackageNotFound_Should_Throw_ProductPackageNotFoundException()
    {
        await Assert.ThrowsAsync<ProductPackageNotFoundException>(
            () => _sut.Handle(new AddPackageItemCommand(99, 101, 1), _ct));
    }

    // ── RemovePackageItemCommand ──────────────────────────────────────────────────

    /// <summary>حذف محصول از پکیج باید آیتم را بردارد</summary>
    [Fact]
    public async Task RemovePackageItem_When_ItemExists_Should_RemoveIt()
    {
        var pkg = MakePackage();
        pkg.AddItem(10, 101, 1);
        _packageRepo.Seed(pkg);

        var result = await _sut.Handle(new RemovePackageItemCommand(1, 101), _ct);

        Assert.True(result);
        Assert.Empty(pkg.Items);
    }

    /// <summary>خطا: محصول در پکیج نیست → PackageItemNotFoundException</summary>
    [Fact]
    public async Task RemovePackageItem_When_ItemNotFound_Should_Throw()
    {
        var pkg = MakePackage();
        _packageRepo.Seed(pkg);

        await Assert.ThrowsAsync<PackageItemNotFoundException>(
            () => _sut.Handle(new RemovePackageItemCommand(1, 999), _ct));
    }

    // ── ActivatePackageCommand ────────────────────────────────────────────────────

    /// <summary>فعال‌سازی پکیج باید IsActive را true کند</summary>
    [Fact]
    public async Task ActivatePackage_When_PackageExists_And_Valid_Should_Activate()
    {
        var pkg = MakePackage();
        _packageRepo.Seed(pkg);

        var result = await _sut.Handle(new ActivatePackageCommand(1), _ct);

        Assert.True(result);
        Assert.True(pkg.IsActive);
    }

    /// <summary>خطا: پکیج پیدا نشد → ProductPackageNotFoundException</summary>
    [Fact]
    public async Task ActivatePackage_When_NotFound_Should_Throw_ProductPackageNotFoundException()
    {
        await Assert.ThrowsAsync<ProductPackageNotFoundException>(
            () => _sut.Handle(new ActivatePackageCommand(99), _ct));
    }

    // ── DeactivatePackageCommand ──────────────────────────────────────────────────

    /// <summary>غیرفعال‌سازی پکیج فعال باید IsActive را false کند</summary>
    [Fact]
    public async Task DeactivatePackage_When_Active_Should_Deactivate()
    {
        var pkg = MakePackage();
        pkg.Activate();
        _packageRepo.Seed(pkg);

        var result = await _sut.Handle(new DeactivatePackageCommand(1), _ct);

        Assert.True(result);
        Assert.False(pkg.IsActive);
    }

    /// <summary>خطا: پکیج پیدا نشد → ProductPackageNotFoundException</summary>
    [Fact]
    public async Task DeactivatePackage_When_NotFound_Should_Throw_ProductPackageNotFoundException()
    {
        await Assert.ThrowsAsync<ProductPackageNotFoundException>(
            () => _sut.Handle(new DeactivatePackageCommand(99), _ct));
    }
}
