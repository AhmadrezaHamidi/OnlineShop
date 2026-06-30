/// <summary>
/// Integration Tests — ارسال پیامک انبوه (SMS Broadcast)
/// داده‌های تست از DevSeedData: ۳ کاربر با شماره موبایل
/// FakeSmsSender در حالت Testing فقط در حافظه ثبت می‌کند
/// </summary>
namespace Ahmad.OnlineShop.Integration.Tests.Marketing;

[Collection("Integration")]
public sealed class MarketingIntegrationTests : IAsyncLifetime
{
    private readonly HttpClient _client;

    public MarketingIntegrationTests(OnlineShopWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
        => await ApiHelper.AuthorizedAsync(_client, ApiHelper.AdminPhone);

    public Task DisposeAsync() => Task.CompletedTask;

    // ── SendBulkSms ───────────────────────────────────────────────────────────

    [Fact]
    public async Task SendBulkSms_Should_Return_200_With_Count()
    {
        var resp = await _client.PostAsJsonAsync(
            $"{ApiHelper.MarketingUrl}/sms/broadcast",
            new { Message = "تخفیف ویژه تابستانه!" });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        // حداقل ۳ کاربر seed شده باید پیامک دریافت کنند
        Assert.True(data.Value.GetInt32() >= 3);
    }

    [Fact]
    public async Task SendBulkSms_Without_Token_Should_Return_401()
    {
        using var client = new HttpClient { BaseAddress = _client.BaseAddress };

        var resp = await client.PostAsJsonAsync(
            $"{ApiHelper.MarketingUrl}/sms/broadcast",
            new { Message = "بدون احراز هویت" });

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
