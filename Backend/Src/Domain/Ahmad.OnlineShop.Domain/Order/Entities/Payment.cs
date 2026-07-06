using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Order.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Entities;

public sealed class Payment : TEntity<long>
{
    public long          OrderId         { get; private set; }
    public decimal       Amount          { get; private set; }
    public PaymentStatus Status          { get; private set; }
    public PaymentMethod Method          { get; private set; }
    public string?       Provider        { get; private set; }
    public string?       Authority       { get; private set; }  // کد زرین‌پال برای redirect
    public string?       TransactionCode { get; private set; }  // کد تراکنش نهایی
    public DateTime?     PaidAt          { get; private set; }

    public bool IsSuccessful => Status == PaymentStatus.Completed;
    public bool IsCashOnDelivery => Method == PaymentMethod.CashOnDelivery;

    private Payment() { }

    internal Payment(long id, long orderId, decimal amount, PaymentMethod method, string? provider)
    {
        GuardAmount(amount);

        Id       = id;
        OrderId  = orderId;
        Amount   = amount;
        Status   = PaymentStatus.Pending;
        Method   = method;
        Provider = provider;
    }

    /// <summary>ثبت Authority زرین‌پال بعد از درخواست پرداخت</summary>
    internal void SetZarinPalAuthority(string authority)
    {
        if (string.IsNullOrWhiteSpace(authority))
            throw new PaymentInvalidAmountException();

        Authority = authority;
    }

    internal void MarkCompleted(string? transactionCode = null)
    {
        Status          = PaymentStatus.Completed;
        TransactionCode = transactionCode;
        PaidAt          = DateTime.UtcNow;
    }

    internal void MarkFailed()    => Status = PaymentStatus.Failed;
    internal void MarkRefunded()  => Status = PaymentStatus.Refunded;

    private static void GuardAmount(decimal amount)
    {
        if (amount <= 0)
            throw new PaymentInvalidAmountException();
    }
}
