using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record RecordPaymentCommand(
    long    OrderId,
    long    PaymentId,
    decimal Amount,
    string? Provider = null
) : ICommand<long>;
