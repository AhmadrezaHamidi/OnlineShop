using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Repositories;
using Order.Domain.Aggregates;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class CreateOrderCommandHandler(IOrderRepository repository)
    : ICommandHandler<CreateOrderCommand, long>
{
    public async Task<long> Handle(CreateOrderCommand command, CancellationToken token)
    {
        var id    = await repository.GetNextIdAsync();
        var order = Order.Create(id, command.UserId, command.PaymentMethod);

        await repository.AddAsync(order, token);
        return order.Id;
    }
}
