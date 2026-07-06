using AhmadBase.Application;
using Ahmad.OnlineShop.Domain.Order.Enums;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record CreateOrderCommand(
    long          UserId,
    PaymentMethod PaymentMethod
) : ICommand<long>;
