using Ahmad.OnlineShop.Domain.Bnpl.Enums;
using Ahmad.OnlineShop.Domain.Bnpl.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Bnpl.Entities;

public sealed class Installment : TEntity<long>
{
    public long ContractId { get; private set; }
    public int InstallmentNo { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public InstallmentStatus Status { get; private set; }

    public bool IsOverdue => Status == InstallmentStatus.Pending && DateTime.UtcNow > DueDate;

    private Installment() { }

    internal Installment(long id, long contractId, int installmentNo, decimal amount, DateTime dueDate)
    {
        GuardAmount(amount);

        Id = id;
        ContractId = contractId;
        InstallmentNo = installmentNo;
        Amount = amount;
        DueDate = dueDate;
        Status = InstallmentStatus.Pending;
    }

    internal void MarkPaid()
    {
        GuardNotAlreadyPaid();
        Status = InstallmentStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    internal void MarkOverdue()
    {
        if (Status == InstallmentStatus.Pending && IsOverdue)
            Status = InstallmentStatus.Overdue;
    }

    internal void Waive()
    {
        GuardNotAlreadyPaid();
        Status = InstallmentStatus.Waived;
        PaidAt = DateTime.UtcNow;
    }

    private static void GuardAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InstallmentInvalidAmountException();
    }

    private void GuardNotAlreadyPaid()
    {
        if (Status == InstallmentStatus.Paid)
            throw new InstallmentAlreadyPaidException();
    }
}
