/// <summary>
/// Integration Tests — ارسال پیامک انبوه (SMS Broadcast)
/// داده‌های تست از DevSeedData: ۳ کاربر با شماره موبایل
/// FakeSmsSender در حالت Testing فقط در حافظه ثبت می‌کند
/// </summary>
namespace Ahmad.OnlineShop.Integration.Tests.Marketing;

[Collection("Integration")]
public sealed class MarketingIntegrationTests : IAsyncLifetime
{
    private readonly HttpClient         _client;
    private readonly OnlineShopWebFactory _factory;

    public MarketingIntegrationTests(OnlineShopWebFactory factory)
    {
        _factory = factory;
        _client  = factory.CreateClient();
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
        Assert.True(data.Value.GetInt32() >= 3);
    }

    [Fact]
    public async Task SendBulkSms_Without_Token_Should_Return_401()
    {
        // باید از factory.CreateClient() استفاده شود نه new HttpClient()
        // چون test server از in-memory transport استفاده می‌کند
        using var unauthClient = _factory.CreateClient();

        var resp = await unauthClient.PostAsJsonAsync(
            $"{ApiHelper.MarketingUrl}/sms/broadcast",
            new { Message = "بدون احراز هویت" });

        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
