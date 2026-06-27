using Ahmad.OnlineShop.Application.Contract.Order.Commands;
using Ahmad.OnlineShop.Domain.Order.Args;

namespace Ahmad.OnlineShop.Application.Order.Mapper;

public static class OrderMapper
{
    public static CreateOrderArg Map(this CreateOrderCommand command, long id)
        => new(
            Id: id,
            UserId: command.UserId,
            PaymentMethod: command.PaymentMethod);
}
