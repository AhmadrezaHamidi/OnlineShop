/// <summary>
/// Integration Tests — احراز هویت با OTP
/// جریان: RequestOtp → VerifyOtp → JWT
/// داده‌های تست از DevSeedData: شماره‌های 09000000001/2/3 ، OTP: 000000
/// </summary>
namespace Ahmad.OnlineShop.Integration.Tests.Auth;

[Collection("Integration")]
public sealed class AuthIntegrationTests(OnlineShopWebFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    // ── RequestOtp ────────────────────────────────────────────────────────────

    [Fact]
    public async Task RequestOtp_With_ValidPhone_Should_Return_200()
    {
        var resp = await _client.PostAsJsonAsync(
            ApiHelper.RequestOtpUrl, new { PhoneNumber = ApiHelper.AdminPhone });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task RequestOtp_For_NewPhone_Should_Create_User_And_Return_200()
    {
        var resp = await _client.PostAsJsonAsync(
            ApiHelper.RequestOtpUrl, new { PhoneNumber = "09900000099" });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    // ── VerifyOtp ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task VerifyOtp_With_CorrectCode_Should_Return_AccessToken()
    {
        await _client.PostAsJsonAsync(ApiHelper.RequestOtpUrl,
            new { PhoneNumber = ApiHelper.CustomerPhone });

        var resp = await _client.PostAsJsonAsync(ApiHelper.VerifyOtpUrl,
            new { PhoneNumber = ApiHelper.CustomerPhone, Code = ApiHelper.OtpCode });

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var body = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var data = ApiHelper.GetData(body);
        Assert.NotNull(data);
        Assert.True(data.Value.TryGetProperty("accessToken", out var token));
        Assert.False(string.IsNullOrEmpty(token.GetString()));
    }

    [Fact]
    public async Task VerifyOtp_With_WrongCode_Should_Return_Error()
    {
        await _client.PostAsJsonAsync(ApiHelper.RequestOtpUrl,
            new { PhoneNumber = ApiHelper.SellerPhone });

        var resp = await _client.PostAsJsonAsync(ApiHelper.VerifyOtpUrl,
            new { PhoneNumber = ApiHelper.SellerPhone, Code = "999999" });

        Assert.NotEqual(HttpStatusCode.OK, resp.StatusCode);
    }

    // ── Protected endpoint بدون Token ─────────────────────────────────────────

    [Fact]
    public async Task Discount_Without_Token_Should_Return_401()
    {
        var resp = await _client.GetAsync(ApiHelper.DiscountsUrl);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    // ── Protected endpoint با Token ───────────────────────────────────────────

    [Fact]
    public async Task Discount_With_Valid_Token_Should_Return_200()
    {
        await ApiHelper.AuthorizedAsync(_client, ApiHelper.AdminPhone);

        var resp = await _client.GetAsync(ApiHelper.DiscountsUrl);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
