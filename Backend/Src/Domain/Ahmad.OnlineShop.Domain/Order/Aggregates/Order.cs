using Ahmad.OnlineShop.Domain.Order.Args;
using Ahmad.OnlineShop.Domain.Order.Entities;
using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Order.Events;
using Ahmad.OnlineShop.Domain.Order.Exceptions;

namespace Ahmad.OnlineShop.Domain.Order.Aggregates;

public sealed class Order : AggregateRoot<long>
{
    private readonly List<OrderItem> _items    = [];
    private readonly List<Payment>   _payments = [];

    public long          UserId        { get; private set; }
    public OrderStatus   Status        { get; private set; }
    public decimal       TotalAmount   { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime      PlacedAt      { get; private set; }
    public DateTime      CreatedAt     { get; private set; }

    public IReadOnlyCollection<OrderItem> Items    => _items.AsReadOnly();
    public IReadOnlyCollection<Payment>   Payments => _payments.AsReadOnly();

    private Order() { }

    private Order(CreateOrderArg arg) : base(arg.Id)
    {
        UserId        = arg.UserId;
        Status        = OrderStatus.Pending;
        PaymentMethod = arg.PaymentMethod;
        TotalAmount   = 0;
        CreatedAt     = DateTime.UtcNow;
        PlacedAt      = DateTime.UtcNow;
    }

    public static Order Create(CreateOrderArg arg)
    {
        var order = new Order(arg);
        order.RaiseDomainEvent(new OrderCreatedEvent(arg.Id, arg.UserId, (int)arg.PaymentMethod));
        return order;
    }

    // ── Items ─────────────────────────────────────────────

    public OrderItem AddItem(AddOrderItemArg arg)
    {
        GuardStatusIsPending();

        var item = new OrderItem(arg.ItemId, Id, arg.ProductId, arg.Quantity, arg.UnitPrice);
        _items.Add(item);
        RecalculateTotal();

        RaiseDomainEvent(new OrderItemAddedEvent(
            Id, item.Id, arg.ProductId, arg.Quantity, arg.UnitPrice, TotalAmount));

        return item;
    }

    public void RemoveItem(long itemId)
    {
        GuardStatusIsPending();

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        GuardItemExists(item);

        _items.Remove(item!);
        RecalculateTotal();

        RaiseDomainEvent(new OrderItemRemovedEvent(
            Id, itemId, item!.ProductId, item.Quantity, TotalAmount));
    }

    public void Place()
    {
        GuardHasItems();
        GuardStatusIsPending();

        RaiseDomainEvent(new OrderPlacedEvent(
            Id, UserId, TotalAmount, (int)PaymentMethod,
            _items.Select(i => new OrderItemSnapshot(i.ProductId, i.Quantity, i.UnitPrice)).ToList()));
    }

    // ── Payment ───────────────────────────────────────────

    public Payment RecordPayment(RecordPaymentArg arg)
    {
        GuardPaymentAmountMatches(arg.Amount);

        var payment = new Payment(arg.PaymentId, Id, arg.Amount, arg.Method, arg.Provider);
        _payments.Add(payment);
        RaiseDomainEvent(new PaymentRecordedEvent(
            arg.PaymentId, Id, arg.Amount, (int)PaymentStatus.Pending, arg.Provider));
        return payment;
    }

    public void MarkPaymentCompleted(long paymentId)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        GuardPaymentExists(payment);
        payment!.MarkCompleted();
        Confirm();
    }

    public void MarkPaymentFailed(long paymentId)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        GuardPaymentExists(payment);
        payment!.MarkFailed();
    }

    // ── Status Transitions ────────────────────────────────

    public void Confirm()
    {
        GuardHasSuccessfulPayment();
        GuardStatusIsPending();

        Status = OrderStatus.Confirmed;
        RaiseDomainEvent(new OrderConfirmedEvent(Id, UserId, TotalAmount));
    }

    public void Ship()
    {
        GuardStatusIsConfirmed();
        Status = OrderStatus.Shipped;
        RaiseDomainEvent(new OrderShippedEvent(Id, UserId));
    }

    public void Deliver()
    {
        GuardStatusIsShipped();
        Status = OrderStatus.Delivered;
        RaiseDomainEvent(new OrderDeliveredEvent(Id, UserId));
    }

    public void Cancel(string reason)
    {
        GuardNotAlreadyCancelled();
        GuardNotAlreadyDelivered();

        Status = OrderStatus.Cancelled;
        RaiseDomainEvent(new OrderCancelledEvent(
            Id, UserId, reason,
            _items.Select(i => new OrderItemSnapshot(i.ProductId, i.Quantity, i.UnitPrice)).ToList()));
    }

    // ── Guards ────────────────────────────────────────────

    private void GuardStatusIsPending()
    {
        if (Status != OrderStatus.Pending)
            throw new OrderInvalidStatusTransitionException();
    }

    private void GuardStatusIsConfirmed()
    {
        if (Status != OrderStatus.Confirmed)
            throw new OrderInvalidStatusTransitionException();
    }

    private void GuardStatusIsShipped()
    {
        if (Status != OrderStatus.Shipped)
            throw new OrderInvalidStatusTransitionException();
    }

    private void GuardHasItems()
    {
        if (!_items.Any())
            throw new OrderNoItemsException();
    }

    private static void GuardItemExists(OrderItem? item)
    {
        if (item is null) throw new OrderItemNotFoundException();
    }

    private static void GuardPaymentExists(Payment? payment)
    {
        if (payment is null) throw new PaymentNotFoundException();
    }

    private void GuardPaymentAmountMatches(decimal amount)
    {
        if (amount != TotalAmount)
            throw new PaymentAmountMismatchException();
    }

    private void GuardHasSuccessfulPayment()
    {
        if (!_payments.Any(p => p.IsSuccessful))
            throw new OrderCannotConfirmWithoutPaymentException();
    }

    private void GuardNotAlreadyCancelled()
    {
        if (Status == OrderStatus.Cancelled)
            throw new OrderAlreadyCancelledException();
    }

    private void GuardNotAlreadyDelivered()
    {
        if (Status == OrderStatus.Delivered)
            throw new OrderAlreadyDeliveredException();
    }

    private void RecalculateTotal()
        => TotalAmount = _items.Sum(i => i.TotalPrice);
}
