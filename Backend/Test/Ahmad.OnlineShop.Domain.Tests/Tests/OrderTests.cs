using Ahmad.OnlineShop.Domain.Order.Aggregates;
using Ahmad.OnlineShop.Domain.Order.Args;
using Ahmad.OnlineShop.Domain.Order.Entities;
using Ahmad.OnlineShop.Domain.Order.Enums;
using Ahmad.OnlineShop.Domain.Order.Events;
using Ahmad.OnlineShop.Domain.Order.Exceptions;
using OrderAgg = Ahmad.OnlineShop.Domain.Order.Aggregates;



namespace Ahmad.OnlineShop.Domain.Order.Tests;

public class OrderTests
{
    private readonly long _orderId = 1001;
    private readonly long _userId = 500;

    [Fact]
    public void Create_Should_Create_Order_With_Pending_Status()
    {
        var arg = new CreateOrderArg(_orderId, _userId, PaymentMethod.ZarinPal);
        var order = OrderAgg.Order.Create(arg);

        Assert.Equal(_orderId, order.Id);
        Assert.Equal(_userId, order.UserId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(PaymentMethod.ZarinPal, order.PaymentMethod);
        Assert.Equal(0, order.TotalAmount);
        Assert.Empty(order.Items);
        Assert.Empty(order.Payments);
    }

    [Fact]
    public void AddItem_Should_Add_Item_And_Recalculate_Total()
    {
        var order = CreatePendingOrder();
        var itemArg = new AddOrderItemArg(1, 101, 2, 150_000m);

        var item = order.AddItem(itemArg);

        Assert.Single(order.Items);
        Assert.Equal(300_000m, order.TotalAmount);
        Assert.Equal(101, item.ProductId);
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public void RemoveItem_Should_Remove_Item_And_Update_Total()
    {
        var order = CreatePendingOrder();
        var itemArg = new AddOrderItemArg(1, 101, 2, 150_000m);
        order.AddItem(itemArg);

        order.RemoveItem(1);

        Assert.Empty(order.Items);
        Assert.Equal(0, order.TotalAmount);
    }

    [Fact]
    public void Place_Should_Raise_OrderPlacedEvent_When_HasItems()
    {
        var order = CreateOrderWithItems();

        order.Place();

        Assert.Single(order.DomainEvents);
        Assert.IsType<OrderPlacedEvent>(order.DomainEvents.First());
    }

    [Fact]
    public void RecordPayment_Should_Add_Payment_And_Raise_Event()
    {
        var order = CreateOrderWithItems();
        var paymentArg = new RecordPaymentArg(2001, 300_000m, "ZarinPal");

        var payment = order.RecordPayment(paymentArg);

        Assert.Single(order.Payments);
        Assert.Equal(300_000m, payment.Amount);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Single(order.DomainEvents.OfType<PaymentRecordedEvent>());
    }

    [Fact]
    public void MarkPaymentCompleted_Should_Confirm_Order()
    {
        var order = CreateOrderWithItems();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, "ZarinPal"));

        order.MarkPaymentCompleted(payment.Id);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.True(payment.IsSuccessful);
    }

    [Fact]
    public void MarkPaymentFailed_Should_Not_Confirm_Order()
    {
        var order = CreateOrderWithItems();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, "ZarinPal"));

        order.MarkPaymentFailed(payment.Id);

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
    }

    [Fact]
    public void Ship_Should_Change_Status_To_Shipped()
    {
        var order = CreateConfirmedOrder();

        order.Ship();

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void Deliver_Should_Change_Status_To_Delivered()
    {
        var order = CreateShippedOrder();

        order.Deliver();

        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Cancel_Should_Change_Status_To_Cancelled()
    {
        var order = CreatePendingOrder();
        order.AddItem(new AddOrderItemArg(1, 101, 1, 100_000m));

        order.Cancel("انصراف مشتری");

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // ── Exception Tests ─────────────────────────────────────

    [Fact]
    public void AddItem_When_StatusNotPending_Should_Throw_Exception()
    {
        var order = CreateConfirmedOrder();
        Assert.Throws<OrderInvalidStatusTransitionException>(() =>
            order.AddItem(new AddOrderItemArg(1, 101, 1, 100_000m)));
    }

    [Fact]
    public void Place_When_NoItems_Should_Throw_Exception()
    {
        var order = CreatePendingOrder();
        Assert.Throws<OrderNoItemsException>(() => order.Place());
    }

    [Fact]
    public void Confirm_Without_Successful_Payment_Should_Throw_Exception()
    {
        var order = CreateOrderWithItems();
        Assert.Throws<OrderCannotConfirmWithoutPaymentException>(() => order.Confirm());
    }

    [Fact]
    public void RecordPayment_With_Wrong_Amount_Should_Throw_Exception()
    {
        var order = CreateOrderWithItems();
        Assert.Throws<PaymentAmountMismatchException>(() =>
            order.RecordPayment(new RecordPaymentArg(2001, 999_999m, null)));
    }

    // ── Helper Methods ─────────────────────────────────────

    private OrderAgg.Order CreatePendingOrder()
    {
        var arg = new CreateOrderArg(_orderId, _userId, PaymentMethod.ZarinPal);
        return OrderAgg.Order.Create(arg);
    }

    private OrderAgg.Order CreateOrderWithItems()
    {
        var order = CreatePendingOrder();
        order.AddItem(new AddOrderItemArg(1, 101, 2, 150_000m));
        return order;
    }

    private OrderAgg.Order CreateConfirmedOrder()
    {
        var order = CreateOrderWithItems();
        order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, "ZarinPal"));
        order.MarkPaymentCompleted(2001);
        return order;
    }

    private OrderAgg.Order CreateShippedOrder()
    {
        var order = CreateConfirmedOrder();
        order.Ship();
        return order;
    }
}

