/// <summary>
/// Integration Tests — کد تخفیف (Discount)
/// جریان کامل: ایجاد → دریافت → فعال/غیرفعال → اعمال روی سفارش
/// داده واقعی در Ahmad.OnlineShopDb_IT ذخیره می‌شود
/// </summary>
namespace Ahmad.OnlineShop.Integration.Tests.Discount;

[Collection("Integration")]
public sealed class DiscountIntegrationTests : IAsyncLifetime
{
    private readonly HttpClient _client;

    public DiscountIntegrationTests(OnlineShopWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
        => await ApiHelper.AuthorizedAsync(_client);

    public Task DisposeAsync() => Task.CompletedTask;

    // ── کد یکتا برای هر اجرا تا conflict نشود ────────────────────────────────
    private static string UniqueCode() => $"IT{DateTime.UtcNow.Ticks % 100000:D5}";

    // ── ایجاد تخفیف ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateDiscount_Should_Return_200_And_Id_In_Db()
    {
        var code = UniqueCode();
        var resp = await _client.PostAsJsonAsync(ApiHelper.DiscountsUrl, new
        {
            Code          = code,
            Title         = "تخفیف یکپارچه‌سازی",
            Type          = (int)DiscountType.Percentage,
            Value         = 10m,
            MinOrderAmount = (decimal?)null,
            MaxUsage      = (int?)null,
            ExpiresAt     = (DateTime?)null
        });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        // data باید یک عدد (ID) باشد
        Assert.True(data.Value.ValueKind == JsonValueKind.Number);
        Assert.True(data.Value.GetInt64() > 0);
    }

    // ── دریافت با کد ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByCode_After_Create_Should_Return_Discount()
    {
        var code = UniqueCode();

        // ایجاد
        await _client.PostAsJsonAsync(ApiHelper.DiscountsUrl, new
        {
            Code = code, Title = "تست دریافت با کد",
            Type = (int)DiscountType.FixedAmount, Value = 50_000m,
            MinOrderAmount = (decimal?)null, MaxUsage = (int?)null, ExpiresAt = (DateTime?)null
        });

        // دریافت با کد
        var resp = await _client.GetAsync($"{ApiHelper.DiscountsUrl}/code/{code}");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        Assert.Equal(code, data.Value.GetProperty("code").GetString());
    }

    // ── فعال/غیرفعال ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeactivateThenActivate_Should_Change_Status()
    {
        var code = UniqueCode();

        // ایجاد
        var createResp = await _client.PostAsJsonAsync(ApiHelper.DiscountsUrl, new
        {
            Code = code, Title = "تست تغییر وضعیت",
            Type = (int)DiscountType.Percentage, Value = 5m,
            MinOrderAmount = (decimal?)null, MaxUsage = (int?)null, ExpiresAt = (DateTime?)null
        });
        var createBody = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = ApiHelper.GetData(createBody)!.Value.GetInt64();

        // غیرفعال
        var deactivateResp = await _client.PutAsync(
            $"{ApiHelper.DiscountsUrl}/{id}/deactivate", null);
        Assert.Equal(HttpStatusCode.OK, deactivateResp.StatusCode);

        // بررسی غیرفعال شدن
        var getResp = await _client.GetAsync($"{ApiHelper.DiscountsUrl}/{id}");
        var getBody = await getResp.Content.ReadFromJsonAsync<JsonElement>();
        var isActive = ApiHelper.GetData(getBody)!.Value.GetProperty("isActive").GetBoolean();
        Assert.False(isActive);

        // فعال کردن
        var activateResp = await _client.PutAsync(
            $"{ApiHelper.DiscountsUrl}/{id}/activate", null);
        Assert.Equal(HttpStatusCode.OK, activateResp.StatusCode);

        // بررسی فعال شدن
        getResp = await _client.GetAsync($"{ApiHelper.DiscountsUrl}/{id}");
        getBody = await getResp.Content.ReadFromJsonAsync<JsonElement>();
        isActive = ApiHelper.GetData(getBody)!.Value.GetProperty("isActive").GetBoolean();
        Assert.True(isActive);
    }

    // ── اعمال تخفیف ───────────────────────────────────────────────────────────

    [Fact]
    public async Task ApplyDiscount_Percentage_Should_Return_Correct_Amount()
    {
        var code = UniqueCode();

        // ایجاد تخفیف ۱۰٪
        await _client.PostAsJsonAsync(ApiHelper.DiscountsUrl, new
        {
            Code = code, Title = "تست اعمال تخفیف",
            Type = (int)DiscountType.Percentage, Value = 10m,
            MinOrderAmount = (decimal?)null, MaxUsage = (int?)null, ExpiresAt = (DateTime?)null
        });

        // اعمال روی سفارش ۱،۰۰۰،۰۰۰ تومان
        var applyResp = await _client.PostAsJsonAsync(
            $"{ApiHelper.DiscountsUrl}/apply",
            new { Code = code, OrderAmount = 1_000_000m });

        Assert.Equal(HttpStatusCode.OK, applyResp.StatusCode);

        var body = await applyResp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        // ۱۰٪ از ۱،۰۰۰،۰۰۰ = ۱۰۰،۰۰۰
        Assert.Equal(100_000m, data.Value.GetDecimal());
    }

    // ── لیست تخفیف‌ها ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetList_Should_Return_200_With_Paged_Result()
    {
        var resp = await _client.GetAsync($"{ApiHelper.DiscountsUrl}?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("data", out _));
    }
}
