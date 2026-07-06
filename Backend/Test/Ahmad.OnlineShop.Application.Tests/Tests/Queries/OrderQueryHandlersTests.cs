/// <summary>
/// ØªØ³Øªâ€ŒÙ‡Ø§ÛŒ Application Query Handler Ø³ÙØ§Ø±Ø´ (OrderQueryHandlers)
/// Ù¾ÙˆØ´Ø´â€ŒØ¯Ù‡Ù†Ø¯Ù‡: Ø¯Ø±ÛŒØ§ÙØª Ø³ÙØ§Ø±Ø´ØŒ Ù„ÛŒØ³Øª Ø³ÙØ§Ø±Ø´Ø§Øª
/// ØªÚ©Ù†ÙˆÙ„ÙˆÚ˜ÛŒ: Fake Repository
/// Ø®Ø·Ø§Ù‡Ø§ÛŒ ØªØ³Øªâ€ŒØ´Ø¯Ù‡: OrderNotFoundException
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
        var order = OrderAgg.Create(new CreateOrderArg(1, 100, PaymentMethod.ZarinPal));
        order.AddItem(new AddOrderItemArg(1, 101, 2, 150_000m));
        return order;
    }

    // â”€â”€â”€ GetOrderQuery â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ø¯Ø±ÛŒØ§ÙØª Ø³ÙØ§Ø±Ø´ Ù…ÙˆØ¬ÙˆØ¯ Ø¨Ø§ÛŒØ¯ Ø§Ø·Ù„Ø§Ø¹Ø§Øª Ú©Ø§Ù…Ù„ Ø¨Ø±Ú¯Ø±Ø¯Ø§Ù†Ø¯</summary>
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

    /// <summary>Ø®Ø·Ø§: Ø³ÙØ§Ø±Ø´ Ù¾ÛŒØ¯Ø§ Ù†Ø´Ø¯ â†’ OrderNotFoundException</summary>
    [Fact]
    public async Task GetOrder_When_NotFound_Should_Throw_OrderNotFoundException()
    {
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => _sut.HandleAsync(new GetOrderQuery(99), _ct));
    }

    // â”€â”€â”€ GetOrdersQuery â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Ù„ÛŒØ³Øª Ø³ÙØ§Ø±Ø´Ø§Øª Ø¨Ø§ÛŒØ¯ Ø¨Ø§ ØµÙØ­Ù‡â€ŒØ¨Ù†Ø¯ÛŒ Ø¨Ø±Ú¯Ø±Ø¯Ø¯</summary>
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

