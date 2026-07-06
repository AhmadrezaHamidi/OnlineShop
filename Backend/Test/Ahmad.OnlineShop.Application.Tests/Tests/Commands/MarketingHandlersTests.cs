/// <summary>
/// تست‌های Application Handler بازاریابی (MarketingHandlers)
/// پوشش‌دهنده: ارسال پیامک انبوه به همه مشتریان
/// تکنولوژی: Fake Repository
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes.Identity;
using Ahmad.OnlineShop.Application.Handlers.Marketing;

namespace Ahmad.OnlineShop.Application.Tests.Commands;

public class MarketingHandlersTests
{
    private readonly FakeSmsService _sms = new();
    private readonly CancellationToken _ct = CancellationToken.None;

    // ── SendBulkSmsCommand ────────────────────────────────────────────────────────

    /// <summary>وقتی مشتری وجود دارد باید پیامک ارسال شود و تعداد را برگرداند</summary>
    [Fact]
    public async Task Send_When_CustomersExist_Should_SendSms_And_ReturnCount()
    {
        var phones = new[] { "09120000001", "09120000002", "09120000003" };
        var userRepo = new FakeUserReadRepository(phones);
        var sut = new MarketingHandlers(userRepo, _sms);

        var result = await sut.Handle(new SendBulkSmsCommand("تخفیف ویژه!"), _ct);

        Assert.Equal(3, result);
        Assert.Equal(3, _sms.LastBulkPhones!.Count);
        Assert.Equal("تخفیف ویژه!", _sms.LastBulkMessage);
    }

    /// <summary>وقتی هیچ مشتری‌ای وجود ندارد باید صفر برگردد و SMS ارسال نشود</summary>
    [Fact]
    public async Task Send_When_NoCustomers_Should_ReturnZero_Without_CallingSms()
    {
        var userRepo = new FakeUserReadRepository();
        var sut = new MarketingHandlers(userRepo, _sms);

        var result = await sut.Handle(new SendBulkSmsCommand("پیام"), _ct);

        Assert.Equal(0, result);
        Assert.Null(_sms.LastBulkPhones);
    }

    /// <summary>ارسال پیامک با یک مشتری باید دقیقاً آن شماره را بفرستد</summary>
    [Fact]
    public async Task Send_When_SingleCustomer_Should_SendToThatPhone()
    {
        var userRepo = new FakeUserReadRepository(["09131234567"]);
        var sut = new MarketingHandlers(userRepo, _sms);

        var result = await sut.Handle(new SendBulkSmsCommand("سلام مشتری"), _ct);

        Assert.Equal(1, result);
        Assert.Single(_sms.LastBulkPhones!);
        Assert.Equal("09131234567", _sms.LastBulkPhones![0]);
    }
}
