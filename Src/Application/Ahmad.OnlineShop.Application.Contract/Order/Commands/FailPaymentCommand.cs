using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record FailPaymentCommand(
    long OrderId,
    long PaymentId
) : ICommand<long>;
