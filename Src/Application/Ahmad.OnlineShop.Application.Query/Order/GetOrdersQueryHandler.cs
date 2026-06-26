using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class GetOrdersQueryHandler(IOrderRepository repository)
    : IQueryHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    public async Task<PagedResult<OrderDto>> HandleAsync(GetOrdersQuery query, CancellationToken token)
    {
        var (orders, total) = await repository.GetListAsync(
            query.Page,
            query.PageSize,
            query.UserId,
            query.Status,
            token);

        var dtos = orders.Select(order => new OrderDto(
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
        )).ToList();

        return new PagedResult<OrderDto>(dtos, total, query.Page, query.PageSize);
    }
}
