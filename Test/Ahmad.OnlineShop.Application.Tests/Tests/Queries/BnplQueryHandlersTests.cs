/// <summary>
/// تست‌های Application Query Handler BNPL (BnplQueryHandlers)
/// پوشش‌دهنده: دریافت قرارداد، اقساط، قراردادهای کاربر، سقف اعتبار
/// تکنولوژی Mock: NSubstitute — IBnplContractRepository, ICreditLimitRepository
/// خطاهای تست‌شده: BnplContractNotFoundException | CreditLimitNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Query.Handlers;
using Ahmad.OnlineShop.Application.Query.Queries;

namespace Ahmad.OnlineShop.Application.Query.Tests;

public class BnplQueryHandlersTests
{
    private readonly IBnplContractRepository _contractRepo = Substitute.For<IBnplContractRepository>();
    private readonly ICreditLimitRepository  _creditRepo   = Substitute.For<ICreditLimitRepository>();
    private readonly BnplQueryHandlers        _sut;
    private readonly CancellationToken        _ct = CancellationToken.None;

    public BnplQueryHandlersTests()
    {
        _sut = new BnplQueryHandlers(_contractRepo, _creditRepo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static BnplContract MakeContract() =>
        BnplContract.Create(new CreateBnplContractArg(1, 10, 100, 900_000m, 3, DateTime.UtcNow.AddDays(30)));

    private static CreditLimit MakeCredit() =>
        CreditLimit.Create(new CreateCreditLimitArg(1, 100, 1_000_000m));

    // ─── GetContractQuery ─────────────────────────────────────────────────────

    /// <summary>دریافت قرارداد موجود باید اطلاعات کامل با اقساط برگرداند</summary>
    [Fact]
    public async Task GetContract_When_Found_Should_ReturnContractWithInstallments()
    {
        var contract = MakeContract();
        _contractRepo.GetByIdAsync(1, _ct).Returns(contract);

        var result = await _sut.HandleAsync(new GetContractQuery(1), _ct);

        Assert.Equal(1,                    result.Id);
        Assert.Equal(900_000m,             result.TotalAmount);
        Assert.Equal(ContractStatus.Active, result.Status);
        Assert.Equal(3,                    result.Installments.Count);
        Assert.False(result.IsCompleted);
    }

    /// <summary>خطا: قرارداد پیدا نشد → BnplContractNotFoundException</summary>
    [Fact]
    public async Task GetContract_When_NotFound_Should_Throw_BnplContractNotFoundException()
    {
        _contractRepo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<BnplContractNotFoundException>(
            () => _sut.HandleAsync(new GetContractQuery(99), _ct));
    }

    // ─── GetInstallmentsQuery ─────────────────────────────────────────────────

    /// <summary>دریافت اقساط باید لیست مرتب‌شده بر اساس شماره قسط برگرداند</summary>
    [Fact]
    public async Task GetInstallments_When_ContractFound_Should_ReturnOrderedInstallments()
    {
        var contract = MakeContract();
        _contractRepo.GetByIdAsync(1, _ct).Returns(contract);

        var result = await _sut.HandleAsync(new GetInstallmentsQuery(1), _ct);

        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].InstallmentNo);
        Assert.Equal(2, result[1].InstallmentNo);
        Assert.Equal(3, result[2].InstallmentNo);
    }

    /// <summary>خطا: قرارداد پیدا نشد برای اقساط → BnplContractNotFoundException</summary>
    [Fact]
    public async Task GetInstallments_When_ContractNotFound_Should_Throw_BnplContractNotFoundException()
    {
        _contractRepo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<BnplContractNotFoundException>(
            () => _sut.HandleAsync(new GetInstallmentsQuery(99), _ct));
    }

    // ─── GetUserContractsQuery ────────────────────────────────────────────────

    /// <summary>دریافت قراردادهای کاربر باید با صفحه‌بندی برگردد</summary>
    [Fact]
    public async Task GetUserContracts_Should_ReturnPagedResult()
    {
        var contracts = new List<BnplContract> { MakeContract(), MakeContract() };
        _contractRepo.GetByUserIdAsync(100, 1, 20, _ct).Returns((contracts, 2));

        var result = await _sut.HandleAsync(new GetUserContractsQuery(100, 1, 20), _ct);

        Assert.Equal(2,  result.Items.Count);
        Assert.Equal(2,  result.TotalCount);
        Assert.Equal(1,  result.Page);
        Assert.Equal(20, result.PageSize);
    }

    // ─── GetCreditLimitQuery ──────────────────────────────────────────────────

    /// <summary>دریافت سقف اعتبار باید اطلاعات صحیح برگرداند</summary>
    [Fact]
    public async Task GetCreditLimit_When_Found_Should_ReturnCreditResponse()
    {
        var credit = MakeCredit();
        credit.Block(300_000m);
        _creditRepo.GetByUserIdAsync(100, _ct).Returns(credit);

        var result = await _sut.HandleAsync(new GetCreditLimitQuery(100), _ct);

        Assert.Equal(100,         result.UserId);
        Assert.Equal(1_000_000m,  result.TotalLimit);
        Assert.Equal(300_000m,    result.UsedLimit);
        Assert.Equal(700_000m,    result.AvailableLimit);
    }

    /// <summary>خطا: سقف اعتبار پیدا نشد → CreditLimitNotFoundException</summary>
    [Fact]
    public async Task GetCreditLimit_When_NotFound_Should_Throw_CreditLimitNotFoundException()
    {
        _creditRepo.GetByUserIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<CreditLimitNotFoundException>(
            () => _sut.HandleAsync(new GetCreditLimitQuery(99), _ct));
    }
}
