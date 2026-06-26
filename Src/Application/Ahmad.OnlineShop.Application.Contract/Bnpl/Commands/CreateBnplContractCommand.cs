using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Commands;

public record CreateBnplContractCommand(
    long     OrderId,
    long     UserId,
    decimal  TotalAmount,
    int      InstallmentCount,
    DateTime FirstDueDate,
    int      IntervalDays = 30
) : ICommand<long>;
