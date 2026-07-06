using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Bnpl.Events;

/// <summary>قرارداد BNPL ایجاد شد — credit باید block بشه</summary>
public sealed record BnplContractCreatedEvent(
    long    ContractId,
    long    OrderId,
    long    UserId,
    decimal TotalAmount,
    int     InstallmentCount
) : IEvent;

/// <summary>قرارداد کامل شد (همه اقساط پرداخت)</summary>
public sealed record BnplContractCompletedEvent(
    long ContractId,
    long UserId
) : IEvent;

/// <summary>قرارداد لغو شد — credit باید آزاد بشه</summary>
public sealed record BnplContractCancelledEvent(
    long    ContractId,
    long    UserId,
    decimal ReleasedAmount
) : IEvent;

/// <summary>قرارداد معوق شد</summary>
public sealed record BnplContractDefaultedEvent(
    long ContractId,
    long UserId
) : IEvent;

/// <summary>قسط پرداخت شد</summary>
public sealed record InstallmentPaidEvent(
    long    ContractId,
    long    InstallmentId,
    int     InstallmentNo,
    decimal Amount,
    long    UserId
) : IEvent;

/// <summary>اعتبار کاربر تغییر کرد</summary>
public sealed record CreditChangedEvent(
    long    UserId,
    decimal OldUsed,
    decimal NewUsed,
    decimal TotalLimit,
    decimal AvailableLimit
) : IEvent;
