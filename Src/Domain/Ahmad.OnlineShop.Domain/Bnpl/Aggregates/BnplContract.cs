using Ahmad.OnlineShop.Domain.Bnpl.Args;
using Ahmad.OnlineShop.Domain.Bnpl.Entities;
using Ahmad.OnlineShop.Domain.Bnpl.Enums;
using Ahmad.OnlineShop.Domain.Bnpl.Events;
using Ahmad.OnlineShop.Domain.Bnpl.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Bnpl.Aggregates;

public sealed class BnplContract : AggregateRoot<long>
{
    private readonly List<Installment> _installments = [];

    public long OrderId { get; private set; }
    public long UserId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public int InstallmentCount { get; private set; }
    public ContractStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Installment> Installments => _installments.AsReadOnly();

    public int PaidCount => _installments.Count(i => i.Status == InstallmentStatus.Paid);
    public bool IsCompleted => PaidCount == InstallmentCount;

    private BnplContract() { }

    private BnplContract(CreateBnplContractArg arg) : base(arg.Id)
    {
        OrderId = arg.OrderId;
        UserId = arg.UserId;
        TotalAmount = arg.TotalAmount;
        InstallmentCount = arg.InstallmentCount;
        Status = ContractStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public static BnplContract Create(CreateBnplContractArg arg)
    {
        GuardTotalAmount(arg.TotalAmount);
        GuardInstallmentCount(arg.InstallmentCount);

        var contract = new BnplContract(arg);
        contract.GenerateInstallments(arg.FirstDueDate, arg.IntervalDays);
        contract.RaiseDomainEvent(new BnplContractCreatedEvent(
            arg.Id, arg.OrderId, arg.UserId, arg.TotalAmount, arg.InstallmentCount));

        return contract;
    }

    public void PayInstallment(long installmentId)
    {
        GuardNotCancelled();
        GuardNotCompleted();

        var inst = _installments.FirstOrDefault(i => i.Id == installmentId);
        GuardInstallmentExists(inst);

        inst!.MarkPaid();
        RaiseDomainEvent(new InstallmentPaidEvent(Id, installmentId, inst.InstallmentNo, inst.Amount, UserId));

        if (IsCompleted)
            Complete();
    }

    public void MarkDefaulted()
    {
        GuardIsActive();
        Status = ContractStatus.Defaulted;
        RaiseDomainEvent(new BnplContractDefaultedEvent(Id, UserId));
    }

    public void Cancel()
    {
        GuardNotCancelled();
        GuardNotCompleted();

        Status = ContractStatus.Cancelled;
        RaiseDomainEvent(new BnplContractCancelledEvent(Id, UserId, TotalAmount));
    }

    public void RefreshOverdueInstallments()
    {
        foreach (var inst in _installments)
            inst.MarkOverdue();
    }

    private void Complete()
    {
        Status = ContractStatus.Completed;
        RaiseDomainEvent(new BnplContractCompletedEvent(Id, UserId));
    }

    private void GenerateInstallments(DateTime firstDueDate, int intervalDays)
    {
        var amount = Math.Round(TotalAmount / InstallmentCount, 2);
        var remainder = TotalAmount - amount * InstallmentCount;

        for (var i = 1; i <= InstallmentCount; i++)
        {
            var installmentAmount = i == InstallmentCount ? amount + remainder : amount;
            var dueDate = firstDueDate.AddDays(intervalDays * (i - 1));
            _installments.Add(new Installment(0, Id, i, installmentAmount, dueDate));
        }
    }

    private static void GuardTotalAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidTotalAmountException();
    }

    private static void GuardInstallmentCount(int count)
    {
        if (count is < 1 or > 48)
            throw new InvalidInstallmentCountException();
    }

    private void GuardNotCancelled()
    {
        if (Status == ContractStatus.Cancelled)
            throw new BnplContractAlreadyCancelledException();
    }

    private void GuardNotCompleted()
    {
        if (Status == ContractStatus.Completed)
            throw new BnplContractAlreadyCompletedException();
    }

    private void GuardIsActive()
    {
        if (Status != ContractStatus.Active)
            throw new BnplContractNotActiveException();
    }

    private static void GuardInstallmentExists(Installment? inst)
    {
        if (inst is null)
            throw new InstallmentNotFoundException();
    }
}
