/// <summary>
/// تست‌های Application Handler BNPL (BnplHandlers)
/// پوشش‌دهنده: ایجاد قرارداد، پرداخت قسط، لغو، پیش‌فرض، عملیات اعتبار
/// تکنولوژی: Fake Repository
/// خطاهای تست‌شده: CreditLimitNotFoundException | BnplContractNotFoundException | InstallmentNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Handlers;
using Ahmad.OnlineShop.Persistence.EF;

namespace Ahmad.OnlineShop.Application.Tests.Commands;

public class BnplHandlersTests
{
    private readonly FakeBnplContractRepository _contractRepo = new();
    private readonly FakeCreditLimitRepository  _creditRepo   = new();
    private readonly BnplHandlers               _sut;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public BnplHandlersTests()
    {
        _sut = new BnplHandlers(_contractRepo, _creditRepo, FakeAppDb.Create());
    }

    private static CreditLimit MakeCredit(decimal total = 1_000_000m) =>
        CreditLimit.Create(new CreateCreditLimitArg(1, 100, total));

    private static BnplContract MakeContract()
        => BnplContract.Create(new CreateBnplContractArg(1, 10, 100, 900_000m, 3, DateTime.UtcNow.AddDays(30)));

    // ─── CreateBnplContractCommand ────────────────────────────────────────────

    /// <summary>ایجاد قرارداد باید اعتبار را مسدود و قرارداد را ذخیره کند</summary>
    [Fact]
    public async Task Create_When_CreditExists_Should_BlockCredit_And_AddContract()
    {
        var credit = MakeCredit(1_000_000m);
        _creditRepo.Seed(credit);

        var cmd = new CreateBnplContractCommand(10, 100, 900_000m, 3, DateTime.UtcNow.AddDays(30));
        await _sut.Handle(cmd, _ct);

        Assert.Equal(900_000m, credit.UsedLimit);
        Assert.NotNull(_contractRepo.Added);
    }

    /// <summary>خطا: اعتبار کاربر پیدا نشد → CreditLimitNotFoundException</summary>
    [Fact]
    public async Task Create_When_CreditNotFound_Should_Throw_CreditLimitNotFoundException()
    {
        var cmd = new CreateBnplContractCommand(10, 100, 900_000m, 3, DateTime.UtcNow.AddDays(30));

        await Assert.ThrowsAsync<CreditLimitNotFoundException>(() => _sut.Handle(cmd, _ct));
    }

    // ─── PayInstallmentCommand ────────────────────────────────────────────────

    /// <summary>پرداخت قسط باید آن را Paid کند</summary>
    [Fact]
    public async Task PayInstallment_When_ContractExists_Should_MarkInstallmentPaid()
    {
        var contract = MakeContract();
        var credit   = MakeCredit();
        credit.Block(900_000m);
        _contractRepo.Seed(contract);
        _creditRepo.Seed(credit);
        var installmentId = contract.Installments.First().Id;

        await _sut.Handle(new PayInstallmentCommand(1, installmentId), _ct);

        Assert.Equal(1, contract.PaidCount);
    }

    /// <summary>خطا: قرارداد پیدا نشد → BnplContractNotFoundException</summary>
    [Fact]
    public async Task PayInstallment_When_ContractNotFound_Should_Throw_BnplContractNotFoundException()
    {
        await Assert.ThrowsAsync<BnplContractNotFoundException>(
            () => _sut.Handle(new PayInstallmentCommand(99, 1), _ct));
    }

    /// <summary>خطا: قسط پیدا نشد → InstallmentNotFoundException</summary>
    [Fact]
    public async Task PayInstallment_When_InstallmentNotFound_Should_Throw_InstallmentNotFoundException()
    {
        var contract = MakeContract();
        _contractRepo.Seed(contract);

        await Assert.ThrowsAsync<InstallmentNotFoundException>(
            () => _sut.Handle(new PayInstallmentCommand(1, 9999), _ct));
    }

    // ─── MarkContractDefaultedCommand ─────────────────────────────────────────

    /// <summary>اعلام پیش‌فرض باید وضعیت قرارداد را Defaulted کند</summary>
    [Fact]
    public async Task MarkDefaulted_When_ContractExists_Should_SetStatusDefaulted()
    {
        var contract = MakeContract();
        _contractRepo.Seed(contract);

        await _sut.Handle(new MarkContractDefaultedCommand(1), _ct);

        Assert.Equal(ContractStatus.Defaulted, contract.Status);
    }

    // ─── CancelBnplContractCommand ────────────────────────────────────────────

    /// <summary>لغو قرارداد باید وضعیت را Cancelled کند</summary>
    [Fact]
    public async Task Cancel_When_ContractExists_Should_SetStatusCancelled()
    {
        var contract = MakeContract();
        _contractRepo.Seed(contract);

        await _sut.Handle(new CancelBnplContractCommand(1), _ct);

        Assert.Equal(ContractStatus.Cancelled, contract.Status);
    }

    // ─── IncreaseCreditLimitCommand ───────────────────────────────────────────

    /// <summary>افزایش سقف اعتبار باید TotalLimit را آپدیت کند</summary>
    [Fact]
    public async Task IncreaseCredit_When_CreditExists_Should_IncreaseTotalLimit()
    {
        var credit = MakeCredit(1_000_000m);
        _creditRepo.Seed(credit);

        await _sut.Handle(new IncreaseCreditLimitCommand(100, 2_000_000m), _ct);

        Assert.Equal(2_000_000m, credit.TotalLimit);
    }

    /// <summary>خطا: اعتبار پیدا نشد → CreditLimitNotFoundException</summary>
    [Fact]
    public async Task IncreaseCredit_When_CreditNotFound_Should_Throw_CreditLimitNotFoundException()
    {
        await Assert.ThrowsAsync<CreditLimitNotFoundException>(
            () => _sut.Handle(new IncreaseCreditLimitCommand(100, 2_000_000m), _ct));
    }

    // ─── BlockCreditCommand ───────────────────────────────────────────────────

    /// <summary>مسدودسازی اعتبار باید UsedLimit را افزایش دهد</summary>
    [Fact]
    public async Task BlockCredit_Should_IncreaseUsedLimit()
    {
        var credit = MakeCredit(1_000_000m);
        _creditRepo.Seed(credit);

        await _sut.Handle(new BlockCreditCommand(100, 300_000m), _ct);

        Assert.Equal(300_000m, credit.UsedLimit);
    }

    // ─── ReleaseCreditCommand ─────────────────────────────────────────────────

    /// <summary>آزادسازی اعتبار باید UsedLimit را کاهش دهد</summary>
    [Fact]
    public async Task ReleaseCredit_Should_DecreaseUsedLimit()
    {
        var credit = MakeCredit(1_000_000m);
        credit.Block(500_000m);
        _creditRepo.Seed(credit);

        await _sut.Handle(new ReleaseCreditCommand(100, 200_000m), _ct);

        Assert.Equal(300_000m, credit.UsedLimit);
    }
}
