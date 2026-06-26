using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Repositories;
using Order.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class OrderStatusCommandHandler(IOrderRepository repository)
    : ICommandHandler<ConfirmOrderCommand, long>,
      ICommandHandler<ShipOrderCommand,    long>,
      ICommandHandler<DeliverOrderCommand, long>,
      ICommandHandler<CancelOrderCommand,  long>
{
    public async Task<long> Handle(ConfirmOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.Confirm();
        await repository.UpdateAsync(order, token);
        return order.Id;
    }

    public async Task<long> Handle(ShipOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.Ship();
        await repository.UpdateAsync(order, token);
        return order.Id;
    }

    public async Task<long> Handle(DeliverOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.Deliver();
        await repository.UpdateAsync(order, token);
        return order.Id;
    }

    public async Task<long> Handle(CancelOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.Cancel(command.Reason);
        await repository.UpdateAsync(order, token);
        return order.Id;
    }
}
