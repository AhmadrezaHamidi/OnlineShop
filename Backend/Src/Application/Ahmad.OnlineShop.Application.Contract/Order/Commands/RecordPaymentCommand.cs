using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Application.Contract.Order.Commands;

public record RecordPaymentCommand(
    [property: JsonIgnore] long OrderId,
    long    PaymentId,
    decimal Amount,
    string? Provider = null
) : ICommand<long>;
