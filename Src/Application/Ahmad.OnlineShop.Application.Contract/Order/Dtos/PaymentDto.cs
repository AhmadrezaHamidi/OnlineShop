using Order.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Dtos;

public sealed record PaymentDto(
    long          Id,
    decimal       Amount,
    PaymentStatus Status,
    string?       Provider,
    DateTime?     PaidAt,
    bool          IsSuccessful
);
