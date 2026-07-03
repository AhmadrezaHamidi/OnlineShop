/// <summary>
/// Integration Tests — محصول
/// جریان: ایجاد دسته‌بندی → ایجاد محصول → دریافت → فعال/غیرفعال/بایگانی
/// دسته‌بندی اولیه از DevSeedData: Id=1 "لوازم جانبی"
///
/// نکته مهم: AhmadBase MapPostEndpoint بدون type param مقدار raw برمی‌گرداند
/// مثلاً POST /Product → JSON body فقط یک عدد است: 123 (نه {"data":123})
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

        // MapGetEndpoint<List<T>> → آرایه مستقیم (بدون ApiResponse wrapper)
        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
        Assert.True(body.GetArrayLength() >= 1);
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

        // MapPostEndpoint بدون type → عدد raw (نه {"data":...})
        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Number, body.ValueKind);
        Assert.True(body.GetInt64() > 0);
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
        var id = (await createResp.Content.ReadFromJsonAsync<JsonElement>()).GetInt64();

        var resp = await _client.GetAsync($"{ApiHelper.ProductUrl}/{id}");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        // MapGetEndpoint<T> → object مستقیم (بدون wrapper)
        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(name, body.GetProperty("name").GetString());
        Assert.Equal(25_000_000m, body.GetProperty("price").GetDecimal());
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
        var id = (await createResp.Content.ReadFromJsonAsync<JsonElement>()).GetInt64();

        // MapPatchEndpoint → HTTP PATCH
        var emptyBody = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
        var deactivateResp = await _client.PatchAsync(
            $"{ApiHelper.ProductUrl}/{id}/Deactivate", emptyBody);
        Assert.Equal(HttpStatusCode.OK, deactivateResp.StatusCode);

        var emptyBody2 = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
        var archiveResp = await _client.PatchAsync(
            $"{ApiHelper.ProductUrl}/{id}/Archive", emptyBody2);
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
        var id = (await createResp.Content.ReadFromJsonAsync<JsonElement>()).GetInt64();

        // MapPatchEndpoint → HTTP PATCH
        var content = JsonContent.Create(new { ProductId = id, Quantity = 100 });
        var resp = await _client.PatchAsync(
            $"{ApiHelper.ProductUrl}/{id}/Inventory/Replenish", content);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
