/// <summary>
/// تست‌های Application Query Handler سفارش (OrderQueryHandlers)
/// پوشش‌دهنده: دریافت سفارش، لیست سفارشات
/// تکنولوژی Mock: NSubstitute — IOrderRepository
/// خطاهای تست‌شده: OrderNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Query.Handlers;
using Ahmad.OnlineShop.Application.Query.Queries;

namespace Ahmad.OnlineShop.Application.Query.Tests;

public class OrderQueryHandlersTests
{
    private readonly IOrderRepository  _repo = Substitute.For<IOrderRepository>();
    private readonly OrderQueryHandlers _sut;
    private readonly CancellationToken  _ct = CancellationToken.None;

    public OrderQueryHandlersTests()
    {
        _sut = new OrderQueryHandlers(_repo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static Order MakeOrder()
    {
        var order = Order.Create(new CreateOrderArg(1, 100, PaymentMethod.Online));
        order.AddItem(new AddOrderItemArg(1, 101, 2, 150_000m));
        return order;
    }

    // ─── GetOrderQuery ────────────────────────────────────────────────────────

    /// <summary>دریافت سفارش موجود باید اطلاعات کامل برگرداند</summary>
    [Fact]
    public async Task GetOrder_When_Found_Should_ReturnOrderResponse()
    {
        var order = MakeOrder();
        _repo.GetByIdAsync(1, _ct).Returns(order);

        var result = await _sut.HandleAsync(new GetOrderQuery(1), _ct);

        Assert.Equal(1,                  result.Id);
        Assert.Equal(100,                result.UserId);
        Assert.Equal(300_000m,           result.TotalAmount);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Single(result.Items);
        Assert.Empty(result.Payments);
    }

    /// <summary>خطا: سفارش پیدا نشد → OrderNotFoundException</summary>
    [Fact]
    public async Task GetOrder_When_NotFound_Should_Throw_OrderNotFoundException()
    {
        _repo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => _sut.HandleAsync(new GetOrderQuery(99), _ct));
    }

    // ─── GetOrdersQuery ───────────────────────────────────────────────────────

    /// <summary>لیست سفارشات باید با صفحه‌بندی برگردد</summary>
    [Fact]
    public async Task GetOrders_Should_ReturnPagedResult()
    {
        var orders = new List<Order> { MakeOrder(), MakeOrder() };
        _repo.GetListAsync(1, 20, null, null, _ct).Returns((orders, 2));

        var result = await _sut.HandleAsync(new GetOrdersQuery(1, 20), _ct);

        Assert.Equal(2,  result.Items.Count);
        Assert.Equal(2,  result.TotalCount);
        Assert.Equal(1,  result.Page);
        Assert.Equal(20, result.PageSize);
    }

    /// <summary>فیلتر بر اساس UserId باید به Repository پاس شود</summary>
    [Fact]
    public async Task GetOrders_With_UserId_Filter_Should_PassFilterToRepo()
    {
        _repo.GetListAsync(1, 20, 100L, null, _ct).Returns((new List<Order>(), 0));

        await _sut.HandleAsync(new GetOrdersQuery(1, 20, UserId: 100), _ct);

        await _repo.Received(1).GetListAsync(1, 20, 100L, null, _ct);
    }
}
