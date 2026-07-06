namespace Ahmad.OnlineShop.Domain.Bnpl.Enums;

public enum ContractStatus
{
    Active,
    Completed,   // همه اقساط پرداخت شد
    Defaulted,   // عقب‌افتاده
    Cancelled
}

public enum InstallmentStatus
{
    Pending,
    Paid,
    Overdue,
    Waived      // بخشوده شد
}
