using Ahmad.OnlineShop.Domain.Bnpl.Args;
using Ahmad.OnlineShop.Domain.Bnpl.Events;
using Ahmad.OnlineShop.Domain.Bnpl.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Bnpl.Aggregates;

public sealed class CreditLimit : AggregateRoot<long>
{
    public long UserId { get; private set; }
    public decimal TotalLimit { get; private set; }
    public decimal UsedLimit { get; private set; }
    public decimal AvailableLimit => TotalLimit - UsedLimit;
    public DateTime UpdatedAt { get; private set; }

    private CreditLimit() { }

    private CreditLimit(CreateCreditLimitArg arg) : base(arg.Id)
    {
        UserId = arg.UserId;
        TotalLimit = arg.TotalLimit;
        UsedLimit = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public static CreditLimit Create(CreateCreditLimitArg arg)
    {
        GuardLimitAmount(arg.TotalLimit);
        return new CreditLimit(arg);
    }

    public void Block(decimal amount)
    {
        GuardPositiveAmount(amount);
        GuardSufficientCredit(amount);

        var old = UsedLimit;
        UsedLimit += amount;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CreditChangedEvent(UserId, old, UsedLimit, TotalLimit, AvailableLimit));
    }

    public void Release(decimal amount)
    {
        GuardPositiveAmount(amount);
        GuardEnoughUsedCredit(amount);

        var old = UsedLimit;
        UsedLimit -= amount;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CreditChangedEvent(UserId, old, UsedLimit, TotalLimit, AvailableLimit));
    }

    public void IncreaseTotalLimit(decimal newLimit)
    {
        GuardNewLimitHigherThanCurrent(newLimit);
        TotalLimit = newLimit;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void GuardLimitAmount(decimal limit)
    {
        if (limit <= 0)
            throw new InvalidCreditLimitAmountException();
    }

    private static void GuardPositiveAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidCreditLimitAmountException();
    }

    private void GuardSufficientCredit(decimal amount)
    {
        if (AvailableLimit < amount)
            throw new InsufficientCreditException();
    }

    private void GuardEnoughUsedCredit(decimal amount)
    {
        if (UsedLimit < amount)
            throw new NegativeCreditException();
    }

    private void GuardNewLimitHigherThanCurrent(decimal newLimit)
    {
        if (newLimit <= TotalLimit)
            throw new InvalidCreditLimitAmountException();
    }
}
