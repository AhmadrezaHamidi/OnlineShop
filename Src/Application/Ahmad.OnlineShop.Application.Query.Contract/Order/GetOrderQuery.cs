using Ahmad.OnlineShop.Domain.Order.Enums;
using AhmadBase.Application.Query;

namespace Ahmad.OnlineShop.Application.Query.Queries;

public record GetOrderQuery(long Id) : IQuery<GetOrderQueryResponse>;

public sealed record GetOrderQueryResponse(
    long                        Id,
    long                        UserId,
    OrderStatus                 Status,
    decimal                     TotalAmount,
    PaymentMethod               PaymentMethod,
    DateTime                    PlacedAt,
    List<GetOrderItemResponse>  Items,
    List<GetPaymentResponse>    Payments);

public sealed record GetOrderItemResponse(
    long    Id,
    long    ProductId,
    int     Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public sealed record GetPaymentResponse(
    long          Id,
    decimal       Amount,
    PaymentStatus Status,
    string?       Provider,
    DateTime?     PaidAt,
    bool          IsSuccessful);
