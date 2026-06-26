using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Repositories;
using Order.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class PlaceOrderCommandHandler(IOrderRepository repository)
    : ICommandHandler<PlaceOrderCommand, long>
{
    public async Task<long> Handle(PlaceOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.Place();
        await repository.UpdateAsync(order, token);
        return order.Id;
    }
}
