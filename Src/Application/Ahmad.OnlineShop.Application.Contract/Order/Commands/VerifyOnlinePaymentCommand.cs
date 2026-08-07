using AhmadBase.Application;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record VerifyOnlinePaymentCommand(
    long   OrderId,
    long   PaymentId,
    string Authority,
    string Status
) : ICommand<bool>;
