using Ahmad.OnlineShop.Application.Contract.Order.Commands;
using Ahmad.OnlineShop.Application.Order.Mapper;
using Ahmad.OnlineShop.Domain.Order.Args;
using Ahmad.OnlineShop.Domain.Order.Exceptions;
using Ahmad.OnlineShop.Persistence.EF;
using OrderAgg = Ahmad.OnlineShop.Domain.Order;


namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class OrderHandlers(
    IOrderRepository      repository,
    ApplicationDbContext  context) :
    ICommandHandler<CreateOrderCommand, long>,
    ICommandHandler<PlaceOrderCommand, long>,
    ICommandHandler<AddOrderItemCommand, long>,
    ICommandHandler<RemoveOrderItemCommand, long>,
    ICommandHandler<ConfirmOrderCommand, long>,
    ICommandHandler<ShipOrderCommand, long>,
    ICommandHandler<DeliverOrderCommand, long>,
    ICommandHandler<CancelOrderCommand, long>,
    ICommandHandler<RecordPaymentCommand, long>,
    ICommandHandler<CompletePaymentCommand, long>,
    ICommandHandler<FailPaymentCommand, long>
{
    #region Order Commands

    public async Task<long> Handle(CreateOrderCommand command, CancellationToken token)
    {
        var id = await repository.GetNextIdAsync();
        var order = OrderAgg.Aggregates.Order.Create(command.Map(id));

        await repository.AddAsync(order, token);
        await context.SaveChangesAsync(token);
        return order.Id;
    }

    public async Task<long> Handle(PlaceOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.Place();
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return order.Id;
    }

    #endregion

    #region Order Item Commands

    public async Task<long> Handle(AddOrderItemCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        var item = order.AddItem(new AddOrderItemArg(
            ItemId:    await repository.GetNextIdAsync(),
            ProductId: command.ProductId,
            Quantity:  command.Quantity,
            UnitPrice: command.UnitPrice));

        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return item.Id;
    }

    public async Task<long> Handle(RemoveOrderItemCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.RemoveItem(command.ItemId);
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return command.ItemId;
    }

    #endregion

    #region Order Status Commands

    public async Task<long> Handle(ConfirmOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.Confirm();
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return order.Id;
    }

    public async Task<long> Handle(ShipOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.Ship();
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return order.Id;
    }

    public async Task<long> Handle(DeliverOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.Deliver();
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return order.Id;
    }

    public async Task<long> Handle(CancelOrderCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.Cancel(command.Reason);
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return order.Id;
    }

    #endregion

    #region Payment Commands

    public async Task<long> Handle(RecordPaymentCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        var payment = order.RecordPayment(new RecordPaymentArg(command.PaymentId, command.Amount, command.Provider));
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return payment.Id;
    }

    public async Task<long> Handle(CompletePaymentCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.MarkPaymentCompleted(command.PaymentId);
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return command.PaymentId;
    }

    public async Task<long> Handle(FailPaymentCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderNotFoundException();

        order.MarkPaymentFailed(command.PaymentId);
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);
        return command.PaymentId;
    }

    #endregion
}
