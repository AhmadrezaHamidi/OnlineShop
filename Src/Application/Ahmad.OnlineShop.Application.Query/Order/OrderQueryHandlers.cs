using Ahmad.OnlineShop.Domain.Order.Exceptions;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public sealed class OrderQueryHandlers(IOrderRepository repository) :
    IQueryHandler<GetOrderQuery,  GetOrderQueryResponse>,
    IQueryHandler<GetOrdersQuery, QueryPagedResult<GetOrderQueryResponse>>
{
    public async Task<GetOrderQueryResponse> HandleAsync(GetOrderQuery query, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(query.Id, token)
            ?? throw new OrderNotFoundException();

        return order.ToResponse();
    }

    public async Task<QueryPagedResult<GetOrderQueryResponse>> HandleAsync(GetOrdersQuery query, CancellationToken token)
    {
        var (orders, total) = await repository.GetListAsync(
            query.Page, query.PageSize, query.UserId, query.Status, token);

        return new QueryPagedResult<GetOrderQueryResponse>(
            orders.Select(o => o.ToResponse()).ToList(),
            total, query.Page, query.PageSize);
    }
}
