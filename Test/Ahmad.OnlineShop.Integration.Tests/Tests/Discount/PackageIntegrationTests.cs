/// <summary>
/// Integration Tests — پکیج محصول
/// جریان کامل: ایجاد → افزودن محصول → فعال‌سازی → غیرفعال‌سازی
/// داده واقعی در Ahmad.OnlineShopDb_IT ذخیره می‌شود
/// </summary>
namespace Ahmad.OnlineShop.Integration.Tests.Discount;

[Collection("Integration")]
public sealed class PackageIntegrationTests : IAsyncLifetime
{
    private readonly HttpClient _client;

    public PackageIntegrationTests(OnlineShopWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
        => await ApiHelper.AuthorizedAsync(_client);

    public Task DisposeAsync() => Task.CompletedTask;

    private static DateTime FutureDate(int days) => DateTime.UtcNow.AddDays(days);

    // ── ایجاد پکیج ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreatePackage_Should_Return_200_And_Id()
    {
        var resp = await _client.PostAsJsonAsync(ApiHelper.PackagesUrl, new
        {
            Title          = "پکیج تابستانه IT",
            Description    = "توضیحات تست",
            DiscountPercent = 15m,
            ValidFrom      = FutureDate(-1),
            ValidTo        = FutureDate(30)
        });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        Assert.True(data.Value.GetInt64() > 0);
    }

    // ── دریافت پکیج ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPackageById_After_Create_Should_Return_Package()
    {
        // ایجاد
        var createResp = await _client.PostAsJsonAsync(ApiHelper.PackagesUrl, new
        {
            Title = "پکیج دریافت IT", Description = (string?)null,
            DiscountPercent = 20m,
            ValidFrom = FutureDate(-1), ValidTo = FutureDate(15)
        });
        var createBody = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = ApiHelper.GetData(createBody)!.Value.GetInt64();

        // دریافت
        var resp = await _client.GetAsync($"{ApiHelper.PackagesUrl}/{id}");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        Assert.Equal("پکیج دریافت IT", data.Value.GetProperty("title").GetString());
        Assert.Equal(20m, data.Value.GetProperty("discountPercent").GetDecimal());
    }

    // ── افزودن محصول و فعال‌سازی ─────────────────────────────────────────────

    [Fact]
    public async Task AddItem_Then_Activate_Should_Work()
    {
        // ایجاد پکیج
        var createResp = await _client.PostAsJsonAsync(ApiHelper.PackagesUrl, new
        {
            Title = "پکیج آیتم IT", Description = (string?)null,
            DiscountPercent = 10m,
            ValidFrom = FutureDate(-1), ValidTo = FutureDate(20)
        });
        var id = ApiHelper.GetData(
            await createResp.Content.ReadFromJsonAsync<JsonElement>())!.Value.GetInt64();

        // افزودن محصول (productId=1 از seed)
        var addResp = await _client.PostAsJsonAsync(
            $"{ApiHelper.PackagesUrl}/{id}/items",
            new { PackageId = id, ProductId = 1L, Quantity = 2 });
        Assert.Equal(HttpStatusCode.OK, addResp.StatusCode);

        // فعال‌سازی
        var activateResp = await _client.PutAsync(
            $"{ApiHelper.PackagesUrl}/{id}/activate", null);
        Assert.Equal(HttpStatusCode.OK, activateResp.StatusCode);

        // بررسی IsActive
        var getResp = await _client.GetAsync($"{ApiHelper.PackagesUrl}/{id}");
        var getBody = await getResp.Content.ReadFromJsonAsync<JsonElement>();
        var isActive = ApiHelper.GetData(getBody)!.Value.GetProperty("isActive").GetBoolean();
        Assert.True(isActive);
    }

    // ── لیست پکیج‌ها ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPackages_Should_Return_200()
    {
        var resp = await _client.GetAsync($"{ApiHelper.PackagesUrl}?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
