/// <summary>
/// تست‌های Application Query Handler BNPL (BnplQueryHandlers)
/// پوشش‌دهنده: دریافت قرارداد، اقساط، قراردادهای کاربر، سقف اعتبار
/// تکنولوژی: Fake Repository
/// خطاهای تست‌شده: BnplContractNotFoundException | CreditLimitNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Query.Handlers;
using Ahmad.OnlineShop.Application.Query.Queries;

namespace Ahmad.OnlineShop.Application.Tests.Queries;

public class BnplQueryHandlersTests
{
    private readonly FakeBnplContractRepository _contractRepo = new();
    private readonly FakeCreditLimitRepository  _creditRepo   = new();
    private readonly BnplQueryHandlers          _sut;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public BnplQueryHandlersTests()
    {
        _sut = new BnplQueryHandlers(_contractRepo, _creditRepo);
    }

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
        _contractRepo.Seed(contract);

        var result = await _sut.HandleAsync(new GetContractQuery(1), _ct);

        Assert.Equal(900_000m,             result.TotalAmount);
        Assert.Equal(ContractStatus.Active, result.Status);
        Assert.Equal(3,                    result.Installments.Count);
        Assert.False(result.IsCompleted);
    }

    /// <summary>خطا: قرارداد پیدا نشد → BnplContractNotFoundException</summary>
    [Fact]
    public async Task GetContract_When_NotFound_Should_Throw_BnplContractNotFoundException()
    {
        await Assert.ThrowsAsync<BnplContractNotFoundException>(
            () => _sut.HandleAsync(new GetContractQuery(99), _ct));
    }

    // ─── GetInstallmentsQuery ─────────────────────────────────────────────────

    /// <summary>دریافت اقساط باید لیست مرتب‌شده برگرداند</summary>
    [Fact]
    public async Task GetInstallments_When_ContractFound_Should_ReturnOrderedInstallments()
    {
        var contract = MakeContract();
        _contractRepo.Seed(contract);

        var result = await _sut.HandleAsync(new GetInstallmentsQuery(1), _ct);

        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].InstallmentNo);
        Assert.Equal(2, result[1].InstallmentNo);
        Assert.Equal(3, result[2].InstallmentNo);
    }

    // ─── GetUserContractsQuery ────────────────────────────────────────────────

    /// <summary>دریافت قراردادهای کاربر باید با صفحه‌بندی برگردد</summary>
    [Fact]
    public async Task GetUserContracts_Should_ReturnPagedResult()
    {
        _contractRepo.Seed(MakeContract());

        var result = await _sut.HandleAsync(new GetUserContractsQuery(100, 1, 20), _ct);

        Assert.Equal(1,  result.Items.Count);
        Assert.Equal(1,  result.TotalCount);
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
        _creditRepo.Seed(credit);

        var result = await _sut.HandleAsync(new GetCreditLimitQuery(100), _ct);

        Assert.Equal(100,        result.UserId);
        Assert.Equal(1_000_000m, result.TotalLimit);
        Assert.Equal(300_000m,   result.UsedLimit);
        Assert.Equal(700_000m,   result.AvailableLimit);
    }

    /// <summary>خطا: سقف اعتبار پیدا نشد → CreditLimitNotFoundException</summary>
    [Fact]
    public async Task GetCreditLimit_When_NotFound_Should_Throw_CreditLimitNotFoundException()
    {
        await Assert.ThrowsAsync<CreditLimitNotFoundException>(
            () => _sut.HandleAsync(new GetCreditLimitQuery(99), _ct));
    }
}
