using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Repositories;
using Order.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class PaymentCommandHandler(IOrderRepository repository)
    : ICommandHandler<RecordPaymentCommand,   long>,
      ICommandHandler<CompletePaymentCommand, long>,
      ICommandHandler<FailPaymentCommand,     long>
{
    public async Task<long> Handle(RecordPaymentCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        var payment = order.RecordPayment(command.PaymentId, command.Amount, command.Provider);
        await repository.UpdateAsync(order, token);
        return payment.Id;
    }

    public async Task<long> Handle(CompletePaymentCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.MarkPaymentCompleted(command.PaymentId);
        await repository.UpdateAsync(order, token);
        return command.PaymentId;
    }

    public async Task<long> Handle(FailPaymentCommand command, CancellationToken token)
    {
        var order = await repository.GetByIdAsync(command.OrderId, token)
            ?? throw new OrderDomainException(OrderErrors.NotFound, OrderErrors.Get(OrderErrors.NotFound).msg);

        order.MarkPaymentFailed(command.PaymentId);
        await repository.UpdateAsync(order, token);
        return command.PaymentId;
    }
}
