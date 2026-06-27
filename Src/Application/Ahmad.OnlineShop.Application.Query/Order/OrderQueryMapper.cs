using Ahmad.OnlineShop.Application.Query.Queries;

namespace Ahmad.OnlineShop.Application.Query.Mappers;

internal static class OrderQueryMapper
{
    internal static GetOrderQueryResponse ToResponse(this Domain.Order.Aggregates.Order order)
        => new(
            order.Id,
            order.UserId,
            order.Status,
            order.TotalAmount,
            order.PaymentMethod,
            order.PlacedAt,
            order.Items.Select(i => i.ToResponse()).ToList(),
            order.Payments.Select(p => p.ToResponse()).ToList());

    internal static GetOrderItemResponse ToResponse(this Domain.Order.Entities.OrderItem i)
        => new(i.Id, i.ProductId, i.Quantity, i.UnitPrice, i.TotalPrice);

    internal static GetPaymentResponse ToResponse(this Domain.Order.Entities.Payment p)
        => new(p.Id, p.Amount, p.Status, p.Provider, p.PaidAt, p.IsSuccessful);
}
