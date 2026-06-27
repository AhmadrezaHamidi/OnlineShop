/// <summary>
/// ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Application Handler Ø³ÙØ§Ø±Ø´ (OrderHandlers)
/// Ù¾ÙˆØ´Ø´â€ŒØ¯Ù‡Ù†Ø¯Ù‡: Ø§ÛŒØ¬Ø§Ø¯ØŒ Ø§ÙØ²ÙˆØ¯Ù†/Ø­Ø°Ù Ø¢ÛŒØªÙ…ØŒ Ú†Ø±Ø®Ù‡ ÙˆØ¶Ø¹ÛŒØªØŒ Ù¾Ø±Ø¯Ø§Ø®Øª
/// ØªÚ©Ù†ÙˆÙ„ÙˆÚ˜ÛŒ: Fake Repository (Ø¨Ø¯ÙˆÙ† Ù†ÛŒØ§Ø² Ø¨Ù‡ Ù…Ú©ÛŒÙ†Ú¯ library)
/// Ø®Ø·Ø§Ù‡Ø§ÛŒ ØªØ³Øªâ€ŒØ´Ø¯Ù‡: OrderNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Order.Mapper;
using Ahmad.OnlineShop.Application.Handlers;

namespace Ahmad.OnlineShop.Application.Tests.Commands;

public class OrderHandlersTests
{
    private readonly FakeOrderRepository _repo = new();
    private readonly OrderHandlers       _sut;
    private readonly CancellationToken   _ct = CancellationToken.None;

    public OrderHandlersTests()
    {
        _sut = new OrderHandlers(_repo);
    }

    // â”€â”€â”€ Helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    private static OrderAgg MakePendingOrder()
        => OrderAgg.Create(new CreateOrderArg(1, 100, PaymentMethod.ZarinPal));

    private static OrderAgg MakeOrderWithItem()
    {
        var order = MakePendingOrder();
        order.AddItem(new AddOrderItemArg(1, 101, 2, 150_000m));
        return order;
    }

    private static OrderAgg MakeConfirmedOrder()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        order.MarkPaymentCompleted(payment.Id);
        return order;
    }

    // â”€â”€â”€ CreateOrderCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø§ÛŒØ¬Ø§Ø¯ Ø³ÙØ§Ø±Ø´ Ø¨Ø§ÛŒØ¯ Ø¢Ù† Ø±Ø§ Ø¯Ø± Repository Ø°Ø®ÛŒØ±Ù‡ Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Create_Should_AddOrder_And_ReturnId()
    {
        var result = await _sut.Handle(
            new CreateOrderCommand(100, PaymentMethod.ZarinPal), _ct);

        Assert.NotNull(_repo.Added);
        Assert.Equal(100, _repo.Added!.UserId);
    }

    // â”€â”€â”€ PlaceOrderCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø®Ø·Ø§: Ø³ÙØ§Ø±Ø´ Ù¾ÛŒØ¯Ø§ Ù†Ø´Ø¯ â†’ OrderNotFoundException</summary>
    [Fact]
    public async Task Place_When_OrderNotFound_Should_Throw_OrderNotFoundException()
    {
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => _sut.Handle(new PlaceOrderCommand(99), _ct));
    }

    // â”€â”€â”€ AddOrderItemCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø§ÙØ²ÙˆØ¯Ù† Ø¢ÛŒØªÙ… Ø¨Ø§ÛŒØ¯ TotalAmount Ø³ÙØ§Ø±Ø´ Ø±Ø§ Ø¢Ù¾Ø¯ÛŒØª Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task AddItem_Should_AddItem_And_UpdateTotal()
    {
        var order = MakePendingOrder();
        _repo.Seed(order);

        await _sut.Handle(new AddOrderItemCommand(1, 101, 2, 150_000m), _ct);

        Assert.Equal(300_000m, order.TotalAmount);
        Assert.Single(order.Items);
    }

    // â”€â”€â”€ RemoveOrderItemCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø­Ø°Ù Ø¢ÛŒØªÙ… Ø¨Ø§ÛŒØ¯ Ø¢ÛŒØªÙ… Ø±Ø§ Ø§Ø² Ø³ÙØ§Ø±Ø´ Ù¾Ø§Ú© Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task RemoveItem_Should_RemoveItem_And_UpdateTotal()
    {
        var order  = MakeOrderWithItem();
        _repo.Seed(order);
        var itemId = order.Items.First().Id;

        await _sut.Handle(new RemoveOrderItemCommand(1, itemId), _ct);

        Assert.Empty(order.Items);
        Assert.Equal(0, order.TotalAmount);
    }

    // â”€â”€â”€ ConfirmOrderCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>CompletePayment Ø¨Ø§ÛŒØ¯ Ø³ÙØ§Ø±Ø´ Ø±Ø§ Ø§Ø² Ø·Ø±ÛŒÙ‚ MarkPaymentCompleted ØªØ£ÛŒÛŒØ¯ Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Confirm_Via_CompletePayment_Should_SetStatusConfirmed()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        _repo.Seed(order);

        await _sut.Handle(new CompletePaymentCommand(1, payment.Id), _ct);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    // â”€â”€â”€ ShipOrderCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø§Ø±Ø³Ø§Ù„ Ø³ÙØ§Ø±Ø´ ØªØ£ÛŒÛŒØ¯Ø´Ø¯Ù‡ Ø¨Ø§ÛŒØ¯ ÙˆØ¶Ø¹ÛŒØª Ø±Ø§ Shipped Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Ship_When_OrderConfirmed_Should_SetStatusToShipped()
    {
        var order = MakeConfirmedOrder();
        _repo.Seed(order);

        await _sut.Handle(new ShipOrderCommand(1), _ct);

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    // â”€â”€â”€ DeliverOrderCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>ØªØ­ÙˆÛŒÙ„ Ø³ÙØ§Ø±Ø´ Ø§Ø±Ø³Ø§Ù„â€ŒØ´Ø¯Ù‡ Ø¨Ø§ÛŒØ¯ ÙˆØ¶Ø¹ÛŒØª Ø±Ø§ Delivered Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Deliver_When_OrderShipped_Should_SetStatusToDelivered()
    {
        var order = MakeConfirmedOrder();
        order.Ship();
        _repo.Seed(order);

        await _sut.Handle(new DeliverOrderCommand(1), _ct);

        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    // â”€â”€â”€ CancelOrderCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ù„ØºÙˆ Ø³ÙØ§Ø±Ø´ Pending Ø¨Ø§ÛŒØ¯ ÙˆØ¶Ø¹ÛŒØª Ø±Ø§ Cancelled Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task Cancel_When_OrderPending_Should_SetStatusToCancelled()
    {
        var order = MakeOrderWithItem();
        _repo.Seed(order);

        await _sut.Handle(new CancelOrderCommand(1, "Ø§Ù†ØµØ±Ø§Ù Ù…Ø´ØªØ±ÛŒ"), _ct);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // â”€â”€â”€ RecordPaymentCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø«Ø¨Øª Ù¾Ø±Ø¯Ø§Ø®Øª Ø¨Ø§ÛŒØ¯ Payment Ø±Ø§ Ø¨Ù‡ Ø³ÙØ§Ø±Ø´ Ø§Ø¶Ø§ÙÙ‡ Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task RecordPayment_Should_AddPaymentToOrder()
    {
        var order = MakeOrderWithItem();
        _repo.Seed(order);

        await _sut.Handle(new RecordPaymentCommand(1, 2001, order.TotalAmount, "ZarinPal"), _ct);

        Assert.Single(order.Payments);
    }

    // â”€â”€â”€ CompletePaymentCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>ØªÚ©Ù…ÛŒÙ„ Ù¾Ø±Ø¯Ø§Ø®Øª Ø¨Ø§ÛŒØ¯ Ø³ÙØ§Ø±Ø´ Ø±Ø§ Confirmed Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task CompletePayment_Should_ConfirmOrder()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        _repo.Seed(order);

        await _sut.Handle(new CompletePaymentCommand(1, payment.Id), _ct);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.True(payment.IsSuccessful);
    }

    // â”€â”€â”€ FailPaymentCommand â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø´Ú©Ø³Øª Ù¾Ø±Ø¯Ø§Ø®Øª Ø¨Ø§ÛŒØ¯ ÙˆØ¶Ø¹ÛŒØª Payment Ø±Ø§ Failed Ú©Ù†Ø¯</summary>
    [Fact]
    public async Task FailPayment_Should_MarkPaymentFailed()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        _repo.Seed(order);

        await _sut.Handle(new FailPaymentCommand(1, payment.Id), _ct);

        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(OrderStatus.Pending,  order.Status);
    }
}

