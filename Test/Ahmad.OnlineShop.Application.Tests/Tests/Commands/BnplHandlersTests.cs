/// <summary>
/// تست‌های Application Handler BNPL (BnplHandlers)
/// پوشش‌دهنده: ایجاد قرارداد، پرداخت قسط، لغو، اعمال پیش‌فرض، عملیات اعتبار
/// تکنولوژی Mock: NSubstitute — IBnplContractRepository, ICreditLimitRepository
/// خطاهای تست‌شده: CreditLimitNotFoundException | BnplContractNotFoundException | InstallmentNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Bnpl.Mapper;

namespace Ahmad.OnlineShop.Application.Handlers.Tests;

public class BnplHandlersTests
{
    private readonly IBnplContractRepository _contractRepo = Substitute.For<IBnplContractRepository>();
    private readonly ICreditLimitRepository  _creditRepo   = Substitute.For<ICreditLimitRepository>();
    private readonly BnplHandlers            _sut;
    private readonly CancellationToken       _ct = CancellationToken.None;

    public BnplHandlersTests()
    {
        _sut = new BnplHandlers(_contractRepo, _creditRepo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static CreditLimit MakeCredit(decimal total = 1_000_000m) =>
        CreditLimit.Create(new CreateCreditLimitArg(1, 100, total));

    private static BnplContract MakeContract()
    {
        var arg = new CreateBnplContractArg(1, 10, 100, 900_000m, 3, DateTime.UtcNow.AddDays(30));
        return BnplContract.Create(arg);
    }

    // ─── CreateBnplContractCommand ────────────────────────────────────────────

    /// <summary>ایجاد قرارداد باید اعتبار را مسدود و قرارداد را ذخیره کند</summary>
    [Fact]
    public async Task Create_When_CreditExists_Should_BlockCredit_And_AddContract()
    {
        var credit = MakeCredit(1_000_000m);
        _creditRepo.GetByUserIdAsync(100, _ct).Returns(credit);
        _contractRepo.GetNextIdAsync().Returns(1L);

        var cmd = new CreateBnplContractCommand(10, 100, 900_000m, 3, DateTime.UtcNow.AddDays(30));
        var result = await _sut.Handle(cmd, _ct);

        Assert.Equal(900_000m, credit.UsedLimit);
        await _contractRepo.Received(1).AddAsync(Arg.Any<BnplContract>(), _ct);
        await _creditRepo.Received(1).UpdateAsync(credit, _ct);
    }

    /// <summary>خطا: اعتبار کاربر پیدا نشد → CreditLimitNotFoundException</summary>
    [Fact]
    public async Task Create_When_CreditNotFound_Should_Throw_CreditLimitNotFoundException()
    {
        _creditRepo.GetByUserIdAsync(100, _ct).ReturnsNull();

        var cmd = new CreateBnplContractCommand(10, 100, 900_000m, 3, DateTime.UtcNow.AddDays(30));

        await Assert.ThrowsAsync<CreditLimitNotFoundException>(() => _sut.Handle(cmd, _ct));
    }

    // ─── PayInstallmentCommand ────────────────────────────────────────────────

    /// <summary>پرداخت قسط باید قسط را Paid کند و اعتبار را آزاد سازد</summary>
    [Fact]
    public async Task PayInstallment_When_ContractExists_Should_PayAndReleaseCredit()
    {
        var contract      = MakeContract();
        var credit        = MakeCredit();
        credit.Block(900_000m);
        var installmentId = contract.Installments.First().Id;

        _contractRepo.GetByIdAsync(1, _ct).Returns(contract);
        _creditRepo.GetByUserIdAsync(contract.UserId, _ct).Returns(credit);

        await _sut.Handle(new PayInstallmentCommand(1, installmentId), _ct);

        Assert.Equal(1, contract.PaidCount);
        await _contractRepo.Received(1).UpdateAsync(contract, _ct);
    }

    /// <summary>خطا: قرارداد پیدا نشد → BnplContractNotFoundException</summary>
    [Fact]
    public async Task PayInstallment_When_ContractNotFound_Should_Throw_BnplContractNotFoundException()
    {
        _contractRepo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<BnplContractNotFoundException>(
            () => _sut.Handle(new PayInstallmentCommand(99, 1), _ct));
    }

    /// <summary>خطا: قسط پیدا نشد → InstallmentNotFoundException</summary>
    [Fact]
    public async Task PayInstallment_When_InstallmentNotFound_Should_Throw_InstallmentNotFoundException()
    {
        var contract = MakeContract();
        _contractRepo.GetByIdAsync(1, _ct).Returns(contract);

        await Assert.ThrowsAsync<InstallmentNotFoundException>(
            () => _sut.Handle(new PayInstallmentCommand(1, 9999), _ct));
    }

    // ─── MarkContractDefaultedCommand ─────────────────────────────────────────

    /// <summary>اعلام پیش‌فرض باید وضعیت قرارداد را Defaulted کند</summary>
    [Fact]
    public async Task MarkDefaulted_When_ContractExists_Should_MarkDefaulted()
    {
        var contract = MakeContract();
        _contractRepo.GetByIdAsync(1, _ct).Returns(contract);

        await _sut.Handle(new MarkContractDefaultedCommand(1), _ct);

        Assert.Equal(ContractStatus.Defaulted, contract.Status);
        await _contractRepo.Received(1).UpdateAsync(contract, _ct);
    }

    /// <summary>خطا: قرارداد پیدا نشد → BnplContractNotFoundException</summary>
    [Fact]
    public async Task MarkDefaulted_When_ContractNotFound_Should_Throw_BnplContractNotFoundException()
    {
        _contractRepo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<BnplContractNotFoundException>(
            () => _sut.Handle(new MarkContractDefaultedCommand(99), _ct));
    }

    // ─── CancelBnplContractCommand ────────────────────────────────────────────

    /// <summary>لغو قرارداد باید وضعیت را Cancelled کند</summary>
    [Fact]
    public async Task Cancel_When_ContractExists_Should_Cancel()
    {
        var contract = MakeContract();
        _contractRepo.GetByIdAsync(1, _ct).Returns(contract);

        await _sut.Handle(new CancelBnplContractCommand(1), _ct);

        Assert.Equal(ContractStatus.Cancelled, contract.Status);
        await _contractRepo.Received(1).UpdateAsync(contract, _ct);
    }

    // ─── IncreaseCreditLimitCommand ───────────────────────────────────────────

    /// <summary>افزایش سقف اعتبار باید TotalLimit را آپدیت کند</summary>
    [Fact]
    public async Task IncreaseCredit_When_CreditExists_Should_IncreaseTotalLimit()
    {
        var credit = MakeCredit(1_000_000m);
        _creditRepo.GetByUserIdAsync(100, _ct).Returns(credit);

        await _sut.Handle(new IncreaseCreditLimitCommand(100, 2_000_000m), _ct);

        Assert.Equal(2_000_000m, credit.TotalLimit);
        await _creditRepo.Received(1).UpdateAsync(credit, _ct);
    }

    /// <summary>خطا: اعتبار پیدا نشد → CreditLimitNotFoundException</summary>
    [Fact]
    public async Task IncreaseCredit_When_CreditNotFound_Should_Throw_CreditLimitNotFoundException()
    {
        _creditRepo.GetByUserIdAsync(100, _ct).ReturnsNull();

        await Assert.ThrowsAsync<CreditLimitNotFoundException>(
            () => _sut.Handle(new IncreaseCreditLimitCommand(100, 2_000_000m), _ct));
    }

    // ─── BlockCreditCommand ───────────────────────────────────────────────────

    /// <summary>مسدودسازی اعتبار باید UsedLimit را افزایش دهد</summary>
    [Fact]
    public async Task BlockCredit_When_CreditExists_Should_IncreaseUsedLimit()
    {
        var credit = MakeCredit(1_000_000m);
        _creditRepo.GetByUserIdAsync(100, _ct).Returns(credit);

        await _sut.Handle(new BlockCreditCommand(100, 300_000m), _ct);

        Assert.Equal(300_000m, credit.UsedLimit);
        await _creditRepo.Received(1).UpdateAsync(credit, _ct);
    }

    // ─── ReleaseCreditCommand ─────────────────────────────────────────────────

    /// <summary>آزادسازی اعتبار باید UsedLimit را کاهش دهد</summary>
    [Fact]
    public async Task ReleaseCredit_When_CreditExists_Should_DecreaseUsedLimit()
    {
        var credit = MakeCredit(1_000_000m);
        credit.Block(500_000m);
        _creditRepo.GetByUserIdAsync(100, _ct).Returns(credit);

        await _sut.Handle(new ReleaseCreditCommand(100, 200_000m), _ct);

        Assert.Equal(300_000m, credit.UsedLimit);
        await _creditRepo.Received(1).UpdateAsync(credit, _ct);
    }
}
