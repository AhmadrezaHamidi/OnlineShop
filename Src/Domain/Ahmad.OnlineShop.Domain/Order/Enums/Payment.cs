using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Order.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Order.Entities;

public sealed class Payment : TEntity<long>
{
    public long OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Provider { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public bool IsSuccessful => Status == PaymentStatus.Completed;

    private Payment() { }

    internal Payment(long id, long orderId, decimal amount, string? provider)
    {
        GuardAmount(amount);

        Id = id;
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        Provider = provider;
    }

    internal void MarkCompleted()
    {
        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;
    }

    internal void MarkFailed() => Status = PaymentStatus.Failed;
    internal void MarkRefunded() => Status = PaymentStatus.Refunded;

    private static void GuardAmount(decimal amount)
    {
        if (amount <= 0)
            throw new PaymentInvalidAmountException();
    }
}
