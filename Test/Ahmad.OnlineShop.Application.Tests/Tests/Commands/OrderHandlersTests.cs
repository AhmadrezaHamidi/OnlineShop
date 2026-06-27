/// <summary>
/// تست‌های Application Handler سفارش (OrderHandlers)
/// پوشش‌دهنده: ایجاد، افزودن/حذف آیتم، چرخه وضعیت، پرداخت
/// تکنولوژی: Fake Repository (بدون نیاز به مکینگ library)
/// خطاهای تست‌شده: OrderNotFoundException
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

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static OrderAgg MakePendingOrder()
        => OrderAgg.Create(new CreateOrderArg(1, 100, PaymentMethod.Online));

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

    // ─── CreateOrderCommand ───────────────────────────────────────────────────

    /// <summary>ایجاد سفارش باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task Create_Should_AddOrder_And_ReturnId()
    {
        var result = await _sut.Handle(
            new CreateOrderCommand(100, PaymentMethod.Online), _ct);

        Assert.NotNull(_repo.Added);
        Assert.Equal(100, _repo.Added!.UserId);
    }

    // ─── PlaceOrderCommand ────────────────────────────────────────────────────

    /// <summary>خطا: سفارش پیدا نشد → OrderNotFoundException</summary>
    [Fact]
    public async Task Place_When_OrderNotFound_Should_Throw_OrderNotFoundException()
    {
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => _sut.Handle(new PlaceOrderCommand(99), _ct));
    }

    // ─── AddOrderItemCommand ──────────────────────────────────────────────────

    /// <summary>افزودن آیتم باید TotalAmount سفارش را آپدیت کند</summary>
    [Fact]
    public async Task AddItem_Should_AddItem_And_UpdateTotal()
    {
        var order = MakePendingOrder();
        _repo.Seed(order);

        await _sut.Handle(new AddOrderItemCommand(1, 101, 2, 150_000m), _ct);

        Assert.Equal(300_000m, order.TotalAmount);
        Assert.Single(order.Items);
    }

    // ─── RemoveOrderItemCommand ───────────────────────────────────────────────

    /// <summary>حذف آیتم باید آیتم را از سفارش پاک کند</summary>
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

    // ─── ConfirmOrderCommand ──────────────────────────────────────────────────

    /// <summary>CompletePayment باید سفارش را از طریق MarkPaymentCompleted تأیید کند</summary>
    [Fact]
    public async Task Confirm_Via_CompletePayment_Should_SetStatusConfirmed()
    {
        var order   = MakeOrderWithItem();
        var payment = order.RecordPayment(new RecordPaymentArg(2001, order.TotalAmount, null));
        _repo.Seed(order);

        await _sut.Handle(new CompletePaymentCommand(1, payment.Id), _ct);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    // ─── ShipOrderCommand ─────────────────────────────────────────────────────

    /// <summary>ارسال سفارش تأییدشده باید وضعیت را Shipped کند</summary>
    [Fact]
    public async Task Ship_When_OrderConfirmed_Should_SetStatusToShipped()
    {
        var order = MakeConfirmedOrder();
        _repo.Seed(order);

        await _sut.Handle(new ShipOrderCommand(1), _ct);

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    // ─── DeliverOrderCommand ──────────────────────────────────────────────────

    /// <summary>تحویل سفارش ارسال‌شده باید وضعیت را Delivered کند</summary>
    [Fact]
    public async Task Deliver_When_OrderShipped_Should_SetStatusToDelivered()
    {
        var order = MakeConfirmedOrder();
        order.Ship();
        _repo.Seed(order);

        await _sut.Handle(new DeliverOrderCommand(1), _ct);

        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    // ─── CancelOrderCommand ───────────────────────────────────────────────────

    /// <summary>لغو سفارش Pending باید وضعیت را Cancelled کند</summary>
    [Fact]
    public async Task Cancel_When_OrderPending_Should_SetStatusToCancelled()
    {
        var order = MakeOrderWithItem();
        _repo.Seed(order);

        await _sut.Handle(new CancelOrderCommand(1, "انصراف مشتری"), _ct);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    // ─── RecordPaymentCommand ─────────────────────────────────────────────────

    /// <summary>ثبت پرداخت باید Payment را به سفارش اضافه کند</summary>
    [Fact]
    public async Task RecordPayment_Should_AddPaymentToOrder()
    {
        var order = MakeOrderWithItem();
        _repo.Seed(order);

        await _sut.Handle(new RecordPaymentCommand(1, 2001, order.TotalAmount, "ZarinPal"), _ct);

        Assert.Single(order.Payments);
    }

    // ─── CompletePaymentCommand ───────────────────────────────────────────────

    /// <summary>تکمیل پرداخت باید سفارش را Confirmed کند</summary>
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

    // ─── FailPaymentCommand ───────────────────────────────────────────────────

    /// <summary>شکست پرداخت باید وضعیت Payment را Failed کند</summary>
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
