using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record CompletePaymentCommand(
    long OrderId,
    long PaymentId
) : ICommand<long>;
