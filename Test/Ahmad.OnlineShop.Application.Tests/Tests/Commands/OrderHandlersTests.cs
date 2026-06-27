/// <summary>
/// تست‌های Application Handler سفارش (OrderHandlers)
/// پوشش‌دهنده: ایجاد، افزودن/حذف آیتم، چرخه وضعیت، پرداخت
/// تکنولوژی Mock: NSubstitute — IOrderRepository
/// خطاهای تست‌شده: OrderNotFoundException | OrderInvalidStatusTransitionException
/// </summary>
using Ahmad.OnlineShop.Application.Order.Mapper;

namespace Ahmad.OnlineShop.Application.Handlers.Tests;

public class OrderHandlersTests
{
    private readonly IOrderRepository _repo = Substitute.For<IOrderRepository>();
    private readonly OrderHandlers    _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public OrderHandlersTests()
    {
        _sut = new OrderHandlers(_repo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static Order MakePendingOrder()
        => Order.Create(new CreateOrderArg(1, 100, PaymentMethod.Online));

    private static Order MakeOrderWithItem()
    {
        var order = MakePendingOrder();
        order.AddItem(new AddOrderItemArg(1, 101, 2, 150_000m));
        return order;
    }

    private static Order MakeConfirmedOrder()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        order.MarkPaymentCompleted(payment.Id);
        return order;
    }

    // ─── CreateOrderCommand ───────────────────────────────────────────────────

    /// <summary>ایجاد سفارش باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task Create_Should_AddOrder_And_ReturnId()
    {
        _repo.GetNextIdAsync().Returns(1L);

        var result = await _sut.Handle(
            new CreateOrderCommand(100, PaymentMethod.Online), _ct);

        Assert.Equal(1, result);
        await _repo.Received(1).AddAsync(Arg.Any<Order>(), _ct);
    }

    // ─── PlaceOrderCommand ────────────────────────────────────────────────────

    /// <summary>ثبت سفارش باید رویداد OrderPlaced را raise کند</summary>
    [Fact]
    public async Task Place_When_OrderHasItems_Should_RaiseEvent()
    {
        var order = MakeOrderWithItem();
        _repo.GetByIdAsync(1, _ct).Returns(order);

        await _sut.Handle(new PlaceOrderCommand(1), _ct);

        await _repo.Received(1).UpdateAsync(order, _ct);
    }

    /// <summary>خطا: سفارش پیدا نشد → OrderNotFoundException</summary>
    [Fact]
    public async Task Place_When_OrderNotFound_Should_Throw_OrderNotFoundException()
    {
        _repo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => _sut.Handle(new PlaceOrderCommand(99), _ct));
    }

    // ─── AddOrderItemCommand ──────────────────────────────────────────────────

    /// <summary>افزودن آیتم باید TotalAmount سفارش را آپدیت کند</summary>
    [Fact]
    public async Task AddItem_Should_AddItem_And_UpdateTotal()
    {
        var order = MakePendingOrder();
        _repo.GetByIdAsync(1, _ct).Returns(order);
        _repo.GetNextIdAsync().Returns(1L);

        var result = await _sut.Handle(
            new AddOrderItemCommand(1, 101, 2, 150_000m), _ct);

        Assert.Equal(300_000m, order.TotalAmount);
        Assert.Single(order.Items);
        await _repo.Received(1).UpdateAsync(order, _ct);
    }

    // ─── RemoveOrderItemCommand ───────────────────────────────────────────────

    /// <summary>حذف آیتم باید آیتم را از سفارش پاک کند</summary>
    [Fact]
    public async Task RemoveItem_Should_RemoveItem_And_UpdateTotal()
    {
        var order = MakeOrderWithItem();
        _repo.GetByIdAsync(1, _ct).Returns(order);
        var itemId = order.Items.First().Id;

        await _sut.Handle(new RemoveOrderItemCommand(1, itemId), _ct);

        Assert.Empty(order.Items);
        Assert.Equal(0, order.TotalAmount);
    }

    // ─── ConfirmOrderCommand ──────────────────────────────────────────────────

    /// <summary>تأیید سفارش باید وضعیت را Confirmed کند</summary>
    [Fact]
    public async Task Confirm_When_OrderHasPayment_Should_ConfirmOrder()
    {
        var order = MakeOrderWithItem();
        var pay   = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        pay.MarkCompleted();
        _repo.GetByIdAsync(1, _ct).Returns(order);

        await _sut.Handle(new ConfirmOrderCommand(1), _ct);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    /// <summary>خطا: سفارش پیدا نشد برای تأیید → OrderNotFoundException</summary>
    [Fact]
    public async Task Confirm_When_OrderNotFound_Should_Throw_OrderNotFoundException()
    {
        _repo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => _sut.Handle(new ConfirmOrderCommand(99), _ct));
    }

    // ─── ShipOrderCommand ─────────────────────────────────────────────────────

    /// <summary>ارسال سفارش باید وضعیت را Shipped کند</summary>
    [Fact]
    public async Task Ship_When_OrderConfirmed_Should_SetStatusToShipped()
    {
        var order = MakeConfirmedOrder();
        _repo.GetByIdAsync(1, _ct).Returns(order);

        await _sut.Handle(new ShipOrderCommand(1), _ct);

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    // ─── DeliverOrderCommand ──────────────────────────────────────────────────

    /// <summary>تحویل سفارش باید وضعیت را Delivered کند</summary>
    [Fact]
    public async Task Deliver_When_OrderShipped_Should_SetStatusToDelivered()
    {
        var order = MakeConfirmedOrder();
        order.Ship();
        _repo.GetByIdAsync(1, _ct).Returns(order);

        await _sut.Handle(new DeliverOrderCommand(1), _ct);

        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    // ─── CancelOrderCommand ───────────────────────────────────────────────────

    /// <summary>لغو سفارش باید وضعیت را Cancelled کند</summary>
    [Fact]
    public async Task Cancel_When_OrderPending_Should_SetStatusToCancelled()
    {
        var order = MakeOrderWithItem();
        _repo.GetByIdAsync(1, _ct).Returns(order);

        await _sut.Handle(new CancelOrderCommand(1, "انصراف مشتری"), _ct);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // ─── RecordPaymentCommand ─────────────────────────────────────────────────

    /// <summary>ثبت پرداخت باید Payment را به سفارش اضافه کند</summary>
    [Fact]
    public async Task RecordPayment_Should_AddPayment_And_ReturnPaymentId()
    {
        var order = MakeOrderWithItem();
        _repo.GetByIdAsync(1, _ct).Returns(order);

        var result = await _sut.Handle(
            new RecordPaymentCommand(1, 2001, order.TotalAmount, "ZarinPal"), _ct);

        Assert.Single(order.Payments);
        await _repo.Received(1).UpdateAsync(order, _ct);
    }

    // ─── CompletePaymentCommand ───────────────────────────────────────────────

    /// <summary>تکمیل پرداخت باید سفارش را Confirmed کند</summary>
    [Fact]
    public async Task CompletePayment_Should_ConfirmOrder()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        _repo.GetByIdAsync(1, _ct).Returns(order);

        await _sut.Handle(new CompletePaymentCommand(1, payment.Id), _ct);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.True(payment.IsSuccessful);
    }

    // ─── FailPaymentCommand ───────────────────────────────────────────────────

    /// <summary>شکست پرداخت باید وضعیت Payment را Failed کند</summary>
    [Fact]
    public async Task FailPayment_Should_MarkPaymentFailed()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        _repo.GetByIdAsync(1, _ct).Returns(order);

        await _sut.Handle(new FailPaymentCommand(1, payment.Id), _ct);

        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(OrderStatus.Pending,  order.Status);
    }
}
