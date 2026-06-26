using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Repositories;
using Order.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class GetOrderQueryHandler(IOrderRepository repository)
    : IQueryHandler<GetOrderQuery, OrderDto>
{
    public async Task<OrderDto> HandleAsync(GetOrderQuery query, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(query.Id, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        return new OrderDto(
            order.Id,
            order.UserId,
            order.Status,
            order.TotalAmount,
            order.PaymentMethod,
            order.PlacedAt,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.ProductId,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice)).ToList(),
            order.Payments.Select(p => new PaymentDto(
                p.Id,
                p.Amount,
                p.Status,
                p.Provider,
                p.PaidAt,
                p.IsSuccessful)).ToList()
        );
    }
}
