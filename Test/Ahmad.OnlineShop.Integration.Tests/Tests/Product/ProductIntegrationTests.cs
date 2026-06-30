/// <summary>
/// Integration Tests — محصول
/// جریان: ایجاد دسته‌بندی → ایجاد محصول → دریافت → فعال/غیرفعال/بایگانی
/// دسته‌بندی اولیه از DevSeedData: Id=1 "لوازم جانبی"
/// </summary>
namespace Ahmad.OnlineShop.Integration.Tests.Product;

[Collection("Integration")]
public sealed class ProductIntegrationTests : IAsyncLifetime
{
    private readonly HttpClient _client;

    public ProductIntegrationTests(OnlineShopWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
        => await ApiHelper.AuthorizedAsync(_client, ApiHelper.SellerPhone);

    public Task DisposeAsync() => Task.CompletedTask;

    // ── دریافت دسته‌بندی‌ها ───────────────────────────────────────────────────

    [Fact]
    public async Task GetCategories_Should_Return_At_Least_One_Category()
    {
        var resp = await _client.GetAsync($"{ApiHelper.ProductUrl}/Categories");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        // دسته‌بندی seed شده باید وجود داشته باشد
        Assert.True(data.Value.GetArrayLength() >= 1);
    }

    // ── ایجاد محصول ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateProduct_With_ValidCategory_Should_Return_Id()
    {
        var resp = await _client.PostAsJsonAsync(ApiHelper.ProductUrl, new
        {
            SellerId    = 101L,
            CategoryId  = 1L,
            Name        = $"لپ‌تاپ تست {DateTime.UtcNow.Ticks % 10000}",
            Description = "محصول تست integration",
            Price       = 50_000_000m
        });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        Assert.True(data.Value.GetInt64() > 0);
    }

    // ── دریافت محصول ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProduct_After_Create_Should_Return_Correct_Data()
    {
        var name = $"محصول دریافت {DateTime.UtcNow.Ticks % 10000}";

        var createResp = await _client.PostAsJsonAsync(ApiHelper.ProductUrl, new
        {
            SellerId = 101L, CategoryId = 1L,
            Name = name, Description = "توضیح", Price = 25_000_000m
        });
        var id = ApiHelper.GetData(
            await createResp.Content.ReadFromJsonAsync<JsonElement>())!.Value.GetInt64();

        var resp = await _client.GetAsync($"{ApiHelper.ProductUrl}/{id}");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        Assert.Equal(name, data.Value.GetProperty("name").GetString());
        Assert.Equal(25_000_000m, data.Value.GetProperty("price").GetDecimal());
    }

    // ── غیرفعال + بایگانی ────────────────────────────────────────────────────

    [Fact]
    public async Task DeactivateThenArchive_Should_Change_Status()
    {
        var createResp = await _client.PostAsJsonAsync(ApiHelper.ProductUrl, new
        {
            SellerId = 101L, CategoryId = 1L,
            Name = $"محصول بایگانی {DateTime.UtcNow.Ticks % 10000}",
            Description = (string?)null, Price = 10_000_000m
        });
        var id = ApiHelper.GetData(
            await createResp.Content.ReadFromJsonAsync<JsonElement>())!.Value.GetInt64();

        // غیرفعال
        var deactivateResp = await _client.PutAsync(
            $"{ApiHelper.ProductUrl}/{id}/Deactivate", null);
        Assert.Equal(HttpStatusCode.OK, deactivateResp.StatusCode);

        // بایگانی
        var archiveResp = await _client.PutAsync(
            $"{ApiHelper.ProductUrl}/{id}/Archive", null);
        Assert.Equal(HttpStatusCode.OK, archiveResp.StatusCode);
    }

    // ── موجودی ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task ReplenishStock_Should_Update_Inventory()
    {
        var createResp = await _client.PostAsJsonAsync(ApiHelper.ProductUrl, new
        {
            SellerId = 101L, CategoryId = 1L,
            Name = $"محصول موجودی {DateTime.UtcNow.Ticks % 10000}",
            Description = (string?)null, Price = 5_000_000m
        });
        var id = ApiHelper.GetData(
            await createResp.Content.ReadFromJsonAsync<JsonElement>())!.Value.GetInt64();

        // افزایش موجودی
        var resp = await _client.PostAsJsonAsync(
            $"{ApiHelper.ProductUrl}/{id}/Inventory/Replenish",
            new { ProductId = id, Quantity = 100 });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
