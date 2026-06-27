/// <summary>
/// تست‌های Application Query Handler سفارش (OrderQueryHandlers)
/// پوشش‌دهنده: دریافت سفارش، لیست سفارشات
/// تکنولوژی: Fake Repository
/// خطاهای تست‌شده: OrderNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Query.Handlers;
using Ahmad.OnlineShop.Application.Query.Queries;

namespace Ahmad.OnlineShop.Application.Tests.Queries;

public class OrderQueryHandlersTests
{
    private readonly FakeOrderRepository _repo = new();
    private readonly OrderQueryHandlers  _sut;
    private readonly CancellationToken   _ct = CancellationToken.None;

    public OrderQueryHandlersTests()
    {
        _sut = new OrderQueryHandlers(_repo);
    }

    private static OrderAgg MakeOrder()
    {
        var order = OrderAgg.Create(new CreateOrderArg(1, 100, PaymentMethod.Online));
        order.AddItem(new AddOrderItemArg(1, 101, 2, 150_000m));
        return order;
    }

    // ─── GetOrderQuery ────────────────────────────────────────────────────────

    /// <summary>دریافت سفارش موجود باید اطلاعات کامل برگرداند</summary>
    [Fact]
    public async Task GetOrder_When_Found_Should_ReturnOrderResponse()
    {
        var order = MakeOrder();
        _repo.Seed(order);

        var result = await _sut.HandleAsync(new GetOrderQuery(1), _ct);

        Assert.Equal(1,                   result.Id);
        Assert.Equal(100,                 result.UserId);
        Assert.Equal(300_000m,            result.TotalAmount);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Single(result.Items);
    }

    /// <summary>خطا: سفارش پیدا نشد → OrderNotFoundException</summary>
    [Fact]
    public async Task GetOrder_When_NotFound_Should_Throw_OrderNotFoundException()
    {
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => _sut.HandleAsync(new GetOrderQuery(99), _ct));
    }

    // ─── GetOrdersQuery ───────────────────────────────────────────────────────

    /// <summary>لیست سفارشات باید با صفحه‌بندی برگردد</summary>
    [Fact]
    public async Task GetOrders_Should_ReturnPagedResult()
    {
        _repo.Seed(MakeOrder());

        var result = await _sut.HandleAsync(new GetOrdersQuery(1, 20), _ct);

        Assert.Equal(1,  result.Items.Count);
        Assert.Equal(1,  result.TotalCount);
        Assert.Equal(1,  result.Page);
        Assert.Equal(20, result.PageSize);
    }
}
