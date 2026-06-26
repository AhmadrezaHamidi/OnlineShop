using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Repositories;
using Order.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class OrderItemCommandHandler(IOrderRepository repository)
    : ICommandHandler<AddOrderItemCommand, long>,
      ICommandHandler<RemoveOrderItemCommand, long>
{
    public async Task<long> Handle(AddOrderItemCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        var item = order.AddItem(
            await GetNextItemIdAsync(),
            command.ProductId,
            command.Quantity,
            command.UnitPrice);

        await repository.UpdateAsync(order, token);
        return item.Id;
    }

    public async Task<long> Handle(RemoveOrderItemCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.RemoveItem(command.ItemId);
        await repository.UpdateAsync(order, token);
        return command.ItemId;
    }

    // Item IDs are derived from the repository's next-id counter scoped per order.
    // If your persistence layer auto-assigns item IDs you can replace this with 0.
    private Task<long> GetNextItemIdAsync()
        => repository.GetNextIdAsync();
}
