namespace Ahmad.OnlineShop.Domain.Bnpl.Args;

public sealed record CreateBnplContractArg(
    long     Id,
    long     OrderId,
    long     UserId,
    decimal  TotalAmount,
    int      InstallmentCount,
    DateTime FirstDueDate,
    int      IntervalDays = 30
);

public sealed record CreateCreditLimitArg(
    long    Id,
    long    UserId,
    decimal TotalLimit
);
