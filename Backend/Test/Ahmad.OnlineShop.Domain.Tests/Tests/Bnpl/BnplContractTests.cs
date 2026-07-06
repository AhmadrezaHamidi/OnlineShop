/// <summary>
/// تست‌های Aggregate Root قرارداد BNPL (BnplContract)
/// پوشش‌دهنده: ایجاد، پرداخت قسط، تکمیل خودکار، پیش‌فرض، لغو
/// خطاهای تست‌شده: InvalidTotalAmountException | InvalidInstallmentCountException
///                  BnplContractAlreadyCancelledException | BnplContractAlreadyCompletedException
///                  BnplContractNotActiveException | InstallmentNotFoundException
/// </summary>
namespace Ahmad.OnlineShop.Domain.Bnpl.Tests;

public class BnplContractTests
{
    private static readonly DateTime FirstDue = DateTime.UtcNow.AddDays(30);

    private static BnplContract CreateContract(
        int     installmentCount = 3,
        decimal totalAmount      = 900_000m) =>
        BnplContract.Create(new CreateBnplContractArg(
            Id:               1,
            OrderId:          10,
            UserId:           100,
            TotalAmount:      totalAmount,
            InstallmentCount: installmentCount,
            FirstDueDate:     FirstDue,
            IntervalDays:     30));

    // ── Create ───────────────────────────────────────────────────────────────

    /// <summary>ایجاد قرارداد باید مشخصات را ست و اقساط را تولید کند</summary>
    [Fact]
    public void Create_Should_Set_Properties_And_GenerateInstallments()
    {
        var contract = CreateContract(installmentCount: 3, totalAmount: 900_000m);

        Assert.Equal(ContractStatus.Active, contract.Status);
        Assert.Equal(3,        contract.InstallmentCount);
        Assert.Equal(900_000m, contract.TotalAmount);
        Assert.Equal(3,        contract.Installments.Count);
        Assert.Equal(0,        contract.PaidCount);
        Assert.False(contract.IsCompleted);
    }

    /// <summary>مجموع مبالغ اقساط باید برابر کل قرارداد باشد</summary>
    [Fact]
    public void Create_Should_Distribute_Installments_Correctly()
    {
        var contract = CreateContract(installmentCount: 3, totalAmount: 900_000m);
        var amounts  = contract.Installments.Select(i => i.Amount).ToList();

        Assert.Equal(900_000m, amounts.Sum());
        Assert.All(amounts, a => Assert.True(a > 0));
    }

    /// <summary>خطا: مبلغ کل صفر → InvalidTotalAmountException</summary>
    [Fact]
    public void Create_When_ZeroAmount_Should_Throw_InvalidTotalAmountException()
    {
        Assert.Throws<InvalidTotalAmountException>(
            () => BnplContract.Create(new CreateBnplContractArg(1, 10, 100, 0, 3, FirstDue)));
    }

    /// <summary>خطا: مبلغ کل منفی → InvalidTotalAmountException</summary>
    [Fact]
    public void Create_When_NegativeAmount_Should_Throw_InvalidTotalAmountException()
    {
        Assert.Throws<InvalidTotalAmountException>(
            () => BnplContract.Create(new CreateBnplContractArg(1, 10, 100, -1000m, 3, FirstDue)));
    }

    /// <summary>خطا: تعداد اقساط صفر → InvalidInstallmentCountException</summary>
    [Fact]
    public void Create_When_ZeroInstallments_Should_Throw_InvalidInstallmentCountException()
    {
        Assert.Throws<InvalidInstallmentCountException>(
            () => BnplContract.Create(new CreateBnplContractArg(1, 10, 100, 900_000m, 0, FirstDue)));
    }

    /// <summary>خطا: تعداد اقساط بیش از ۴۸ → InvalidInstallmentCountException</summary>
    [Fact]
    public void Create_When_InstallmentsExceed48_Should_Throw_InvalidInstallmentCountException()
    {
        Assert.Throws<InvalidInstallmentCountException>(
            () => BnplContract.Create(new CreateBnplContractArg(1, 10, 100, 900_000m, 49, FirstDue)));
    }

    // ── PayInstallment ───────────────────────────────────────────────────────

    /// <summary>پرداخت قسط باید وضعیت را Paid کند و تعداد پرداختی را افزایش دهد</summary>
    [Fact]
    public void PayInstallment_Should_Mark_Installment_Paid_And_Increment_PaidCount()
    {
        var contract      = CreateContract(3);
        var installmentId = contract.Installments.First().Id;
        contract.PayInstallment(installmentId);

        Assert.Equal(1, contract.PaidCount);
        Assert.Equal(InstallmentStatus.Paid,
            contract.Installments.First(i => i.Id == installmentId).Status);
    }

    /// <summary>پرداخت آخرین قسط باید قرارداد را Completed کند</summary>
    [Fact]
    public void PayInstallment_When_AllPaid_Should_CompleteContract()
    {
        var contract = CreateContract(installmentCount: 1, totalAmount: 300_000m);
        var id       = contract.Installments.First().Id;
        contract.PayInstallment(id);

        Assert.True(contract.IsCompleted);
        Assert.Equal(ContractStatus.Completed, contract.Status);
    }

    /// <summary>خطا: قرارداد لغوشده → BnplContractAlreadyCancelledException</summary>
    [Fact]
    public void PayInstallment_When_Cancelled_Should_Throw_BnplContractAlreadyCancelledException()
    {
        var contract = CreateContract();
        contract.Cancel();

        Assert.Throws<BnplContractAlreadyCancelledException>(
            () => contract.PayInstallment(contract.Installments.First().Id));
    }

    /// <summary>خطا: قسط پیدا نشد → InstallmentNotFoundException</summary>
    [Fact]
    public void PayInstallment_When_NotFound_Should_Throw_InstallmentNotFoundException()
    {
        var contract = CreateContract();

        Assert.Throws<InstallmentNotFoundException>(() => contract.PayInstallment(9999));
    }

    // ── MarkDefaulted ────────────────────────────────────────────────────────

    /// <summary>اعلام پیش‌فرض باید وضعیت را Defaulted کند</summary>
    [Fact]
    public void MarkDefaulted_Should_Change_Status_To_Defaulted()
    {
        var contract = CreateContract();
        contract.MarkDefaulted();

        Assert.Equal(ContractStatus.Defaulted, contract.Status);
    }

    /// <summary>خطا: اعلام پیش‌فرض روی قرارداد لغوشده → BnplContractNotActiveException</summary>
    [Fact]
    public void MarkDefaulted_When_Cancelled_Should_Throw_BnplContractNotActiveException()
    {
        var contract = CreateContract();
        contract.Cancel();

        Assert.Throws<BnplContractNotActiveException>(() => contract.MarkDefaulted());
    }

    // ── Cancel ───────────────────────────────────────────────────────────────

    /// <summary>لغو قرارداد باید وضعیت را Cancelled کند</summary>
    [Fact]
    public void Cancel_Should_Change_Status_To_Cancelled()
    {
        var contract = CreateContract();
        contract.Cancel();

        Assert.Equal(ContractStatus.Cancelled, contract.Status);
    }

    /// <summary>خطا: لغو قرارداد از قبل لغوشده → BnplContractAlreadyCancelledException</summary>
    [Fact]
    public void Cancel_When_AlreadyCancelled_Should_Throw_BnplContractAlreadyCancelledException()
    {
        var contract = CreateContract();
        contract.Cancel();

        Assert.Throws<BnplContractAlreadyCancelledException>(() => contract.Cancel());
    }

    /// <summary>خطا: لغو قرارداد تکمیل‌شده → BnplContractAlreadyCompletedException</summary>
    [Fact]
    public void Cancel_When_Completed_Should_Throw_BnplContractAlreadyCompletedException()
    {
        var contract = CreateContract(installmentCount: 1, totalAmount: 300_000m);
        contract.PayInstallment(contract.Installments.First().Id);

        Assert.Throws<BnplContractAlreadyCompletedException>(() => contract.Cancel());
    }

    // ── PaidCount / IsCompleted ──────────────────────────────────────────────

    /// <summary>تعداد اقساط پرداخت‌شده باید صحیح محاسبه شود</summary>
    [Fact]
    public void PaidCount_Reflects_Number_Of_Paid_Installments()
    {
        var contract = CreateContract(installmentCount: 1, totalAmount: 300_000m);
        var id       = contract.Installments.First().Id;
        contract.PayInstallment(id);

        Assert.Equal(1, contract.PaidCount);
        Assert.True(contract.IsCompleted);
    }
}
