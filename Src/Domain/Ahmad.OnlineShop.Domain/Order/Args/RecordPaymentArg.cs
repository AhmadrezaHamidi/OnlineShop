namespace Ahmad.OnlineShop.Domain.Order.Args;

public sealed record RecordPaymentArg(
    long PaymentId,
    decimal Amount,
    string? Provider
);
