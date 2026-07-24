using Ahmad.OnlineShop.Application.Contract.Order.Commands;
using Ahmad.OnlineShop.Application.Contract.Payment;
using Ahmad.OnlineShop.Application.Order.Mapper;
using Ahmad.OnlineShop.Domain.Order.Args;
using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Order.Exceptions;
using Ahmad.OnlineShop.Persistence.EF;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using OrderAgg = Ahmad.OnlineShop.Domain.Order;


namespace Ahmad.OnlineShop.Application.Handlers;

public sealed class OrderHandlers(
    IOrderRepository      repository,
    ApplicationDbContext  context,
    IZarinPalService      zarinpalService,
    ILogger<OrderHandlers>? logger = null) :
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
    ICommandHandler<FailPaymentCommand, long>,
    ICommandHandler<InitiateOnlinePaymentCommand, InitiatePaymentResult>,
    ICommandHandler<VerifyOnlinePaymentCommand, bool>
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

    #region Online Payment — ZarinPal با Fallback به CashOnDelivery

    public async Task<InitiatePaymentResult> Handle(InitiateOnlinePaymentCommand command, CancellationToken token)
    {
        var order     = await repository.GetByIdAsync(command.OrderId, token) ?? throw new OrderNotFoundException();
        var paymentId = await repository.GetNextIdAsync();

        try
        {
            var result = await zarinpalService.RequestAsync(
                order.TotalAmount, command.Description, command.CallbackUrl, token);

            if (!result.Success)
            {
                logger?.LogWarning("زرین‌پال پرداخت رد کرد. خطا: {Error}. Fallback به COD.", result.Error);
                return await FallbackToCashOnDelivery(order, paymentId, result.Error, token);
            }

            order.RecordPayment(new RecordPaymentArg(paymentId, order.TotalAmount, PaymentMethod.ZarinPal, "ZarinPal"));
            order.SetPaymentAuthority(paymentId, result.Authority!);

            await repository.UpdateAsync(order, token);
            await context.SaveChangesAsync(token);

            logger?.LogInformation("پرداخت آنلاین سفارش {OrderId} آغاز شد. Authority: {Authority}", order.Id, result.Authority);
            return new InitiatePaymentResult(PaymentMethod.ZarinPal, paymentId, result.RedirectUrl);
        }
        catch (BrokenCircuitException ex)
        {
            logger?.LogWarning(ex, "Circuit زرین‌پال باز است. سفارش {OrderId} به CashOnDelivery تغییر می‌یابد.", order.Id);
            return await FallbackToCashOnDelivery(order, paymentId, "درگاه پرداخت موقتاً در دسترس نیست", token);
        }
    }

    public async Task<bool> Handle(VerifyOnlinePaymentCommand command, CancellationToken token)
    {
        if (command.Status != "OK")
        {
            logger?.LogWarning("تأیید پرداخت سفارش {OrderId} ناموفق. وضعیت: {Status}", command.OrderId, command.Status);
            return false;
        }

        var order   = await repository.GetByIdAsync(command.OrderId, token) ?? throw new OrderNotFoundException();
        var payment = order.Payments.FirstOrDefault(p => p.Id == command.PaymentId)  ?? throw new PaymentNotFoundException();

        var result = await zarinpalService.VerifyAsync(command.Authority, payment.Amount, token);
        if (!result.Success)
        {
            logger?.LogWarning("تأیید زرین‌پال ناموفق. خطا: {Error}", result.Error);
            return false;
        }

        order.MarkPaymentCompleted(command.PaymentId, result.TransactionCode);
        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);

        logger?.LogInformation("پرداخت سفارش {OrderId} تأیید شد. TransactionCode: {TxCode}", order.Id, result.TransactionCode);
        return true;
    }

    private async Task<InitiatePaymentResult> FallbackToCashOnDelivery(
        OrderAgg.Aggregates.Order order, long paymentId, string? reason, CancellationToken token)
    {
        order.RecordPayment(new RecordPaymentArg(paymentId, order.TotalAmount, PaymentMethod.CashOnDelivery, "COD-Fallback"));
        order.MarkPaymentCompleted(paymentId);

        await repository.UpdateAsync(order, token);
        await context.SaveChangesAsync(token);

        const string msg = "درگاه پرداخت آنلاین موقتاً در دسترس نیست. سفارش به صورت پرداخت درب منزل ثبت شد.";
        return new InitiatePaymentResult(PaymentMethod.CashOnDelivery, paymentId, null, FallbackUsed: true, msg);
    }

    #endregion
}
