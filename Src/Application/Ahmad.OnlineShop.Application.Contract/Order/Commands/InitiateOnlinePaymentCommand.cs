using AhmadBase.Application;
using Ahmad.OnlineShop.Domain.Order.Enums;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record InitiateOnlinePaymentCommand(
    long   OrderId,
    string CallbackUrl,
    string Description = "پرداخت سفارش"
) : ICommand<InitiatePaymentResult>;

public record InitiatePaymentResult(
    PaymentMethod Method,
    long          PaymentId,
    string?       RedirectUrl,
    bool          FallbackUsed    = false,
    string?       FallbackMessage = null
);
