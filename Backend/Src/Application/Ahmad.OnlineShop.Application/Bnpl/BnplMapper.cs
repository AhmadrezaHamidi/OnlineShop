using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Bnpl.Args;

namespace Ahmad.OnlineShop.Application.Bnpl.Mapper;

public static class BnplMapper
{
    public static CreateBnplContractArg Map(this CreateBnplContractCommand command, long id)
        => new(
            Id:               id,
            OrderId:          command.OrderId,
            UserId:           command.UserId,
            TotalAmount:      command.TotalAmount,
            InstallmentCount: command.InstallmentCount,
            FirstDueDate:     command.FirstDueDate,
            IntervalDays:     command.IntervalDays);
}
