using Ahmad.OnlineShop.Domain.Order.Enums;

namespace Ahmad.OnlineShop.Domain.Order.Args;

public sealed record RecordPaymentArg(
    long          PaymentId,
    decimal       Amount,
    string?       Provider,
    PaymentMethod Method = PaymentMethod.ZarinPal
);
