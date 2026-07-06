/// <summary>
/// تست‌های Aggregate Root سقف اعتبار (CreditLimit)
/// پوشش‌دهنده: ایجاد، مسدودسازی، آزادسازی، افزایش سقف، محاسبه اعتبار موجود
/// خطاهای تست‌شده: InvalidCreditLimitAmountException | InsufficientCreditException | NegativeCreditException
/// </summary>
namespace Ahmad.OnlineShop.Domain.Bnpl.Tests;

public class CreditLimitTests
{
    private static CreditLimit CreateCredit(decimal total = 1_000_000m) =>
        CreditLimit.Create(new CreateCreditLimitArg(1, 100, total));

    // ── Create ───────────────────────────────────────────────────────────────

    /// <summary>ایجاد سقف اعتبار باید مقدار کل را ست و UsedLimit را صفر کند</summary>
    [Fact]
    public void Create_Should_Set_TotalLimit_And_Zero_UsedLimit()
    {
        var credit = CreateCredit(500_000m);

        Assert.Equal(500_000m, credit.TotalLimit);
        Assert.Equal(0m,       credit.UsedLimit);
        Assert.Equal(500_000m, credit.AvailableLimit);
    }

    /// <summary>خطا: سقف صفر → InvalidCreditLimitAmountException</summary>
    [Fact]
    public void Create_When_ZeroLimit_Should_Throw_InvalidCreditLimitAmountException()
    {
        Assert.Throws<InvalidCreditLimitAmountException>(
            () => CreditLimit.Create(new CreateCreditLimitArg(1, 100, 0)));
    }

    /// <summary>خطا: سقف منفی → InvalidCreditLimitAmountException</summary>
    [Fact]
    public void Create_When_NegativeLimit_Should_Throw_InvalidCreditLimitAmountException()
    {
        Assert.Throws<InvalidCreditLimitAmountException>(
            () => CreditLimit.Create(new CreateCreditLimitArg(1, 100, -100m)));
    }

    // ── Block ────────────────────────────────────────────────────────────────

    /// <summary>مسدودسازی باید UsedLimit را افزایش دهد</summary>
    [Fact]
    public void Block_Should_Increase_UsedLimit()
    {
        var credit = CreateCredit(1_000_000m);
        credit.Block(300_000m);

        Assert.Equal(300_000m, credit.UsedLimit);
        Assert.Equal(700_000m, credit.AvailableLimit);
    }

    /// <summary>مسدودسازی چندباره باید تجمیع شود</summary>
    [Fact]
    public void Block_Multiple_Times_Should_Accumulate()
    {
        var credit = CreateCredit(1_000_000m);
        credit.Block(200_000m);
        credit.Block(300_000m);

        Assert.Equal(500_000m, credit.UsedLimit);
        Assert.Equal(500_000m, credit.AvailableLimit);
    }

    /// <summary>خطا: اعتبار کافی نیست → InsufficientCreditException</summary>
    [Fact]
    public void Block_When_InsufficientCredit_Should_Throw_InsufficientCreditException()
    {
        var credit = CreateCredit(100_000m);

        Assert.Throws<InsufficientCreditException>(() => credit.Block(200_000m));
    }

    /// <summary>خطا: مقدار مسدود صفر → InvalidCreditLimitAmountException</summary>
    [Fact]
    public void Block_When_ZeroAmount_Should_Throw_InvalidCreditLimitAmountException()
    {
        var credit = CreateCredit();

        Assert.Throws<InvalidCreditLimitAmountException>(() => credit.Block(0));
    }

    /// <summary>خطا: مقدار مسدود منفی → InvalidCreditLimitAmountException</summary>
    [Fact]
    public void Block_When_NegativeAmount_Should_Throw_InvalidCreditLimitAmountException()
    {
        var credit = CreateCredit();

        Assert.Throws<InvalidCreditLimitAmountException>(() => credit.Block(-50_000m));
    }

    // ── Release ──────────────────────────────────────────────────────────────

    /// <summary>آزادسازی باید UsedLimit را کاهش دهد</summary>
    [Fact]
    public void Release_Should_Decrease_UsedLimit()
    {
        var credit = CreateCredit(1_000_000m);
        credit.Block(500_000m);
        credit.Release(200_000m);

        Assert.Equal(300_000m, credit.UsedLimit);
        Assert.Equal(700_000m, credit.AvailableLimit);
    }

    /// <summary>خطا: آزادسازی بیشتر از مقدار مسدود → NegativeCreditException</summary>
    [Fact]
    public void Release_When_MoreThanUsed_Should_Throw_NegativeCreditException()
    {
        var credit = CreateCredit(1_000_000m);
        credit.Block(100_000m);

        Assert.Throws<NegativeCreditException>(() => credit.Release(200_000m));
    }

    /// <summary>خطا: مقدار آزادسازی صفر → InvalidCreditLimitAmountException</summary>
    [Fact]
    public void Release_When_ZeroAmount_Should_Throw_InvalidCreditLimitAmountException()
    {
        var credit = CreateCredit();

        Assert.Throws<InvalidCreditLimitAmountException>(() => credit.Release(0));
    }

    // ── IncreaseTotalLimit ───────────────────────────────────────────────────

    /// <summary>افزایش سقف باید مقدار جدید را ست کند</summary>
    [Fact]
    public void IncreaseTotalLimit_Should_Update_TotalLimit()
    {
        var credit = CreateCredit(1_000_000m);
        credit.IncreaseTotalLimit(2_000_000m);

        Assert.Equal(2_000_000m, credit.TotalLimit);
    }

    /// <summary>خطا: سقف جدید کمتر از سقف فعلی → InvalidCreditLimitAmountException</summary>
    [Fact]
    public void IncreaseTotalLimit_When_LowerThanCurrent_Should_Throw_InvalidCreditLimitAmountException()
    {
        var credit = CreateCredit(1_000_000m);

        Assert.Throws<InvalidCreditLimitAmountException>(() => credit.IncreaseTotalLimit(500_000m));
    }

    /// <summary>خطا: سقف جدید برابر سقف فعلی → InvalidCreditLimitAmountException</summary>
    [Fact]
    public void IncreaseTotalLimit_When_EqualToCurrent_Should_Throw_InvalidCreditLimitAmountException()
    {
        var credit = CreateCredit(1_000_000m);

        Assert.Throws<InvalidCreditLimitAmountException>(() => credit.IncreaseTotalLimit(1_000_000m));
    }

    // ── AvailableLimit computed ──────────────────────────────────────────────

    /// <summary>اعتبار موجود باید برابر سقف کل منهای اعتبار مصرفی باشد</summary>
    [Fact]
    public void AvailableLimit_Should_Equal_TotalLimit_Minus_UsedLimit()
    {
        var credit = CreateCredit(1_000_000m);
        credit.Block(400_000m);

        Assert.Equal(credit.TotalLimit - credit.UsedLimit, credit.AvailableLimit);
    }
}
