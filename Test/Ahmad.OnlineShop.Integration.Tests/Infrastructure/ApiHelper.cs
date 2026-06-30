namespace Ahmad.OnlineShop.Integration.Tests.Infrastructure;

/// <summary>
/// کمک‌کننده‌های مشترک برای Integration Tests
/// شماره‌های تست از DevSeedData seed شده‌اند
/// </summary>
public static class ApiHelper
{
    // ── کاربران seed شده در DevSeedData ───────────────────────────────────────
    public const string AdminPhone    = "09000000001";
    public const string SellerPhone   = "09000000002";
    public const string CustomerPhone = "09000000003";
    public const string OtpCode       = "000000";

    // ── Routes ────────────────────────────────────────────────────────────────
    public const string RequestOtpUrl = "/api/v1/Identity/Auth/RequestOtp";
    public const string VerifyOtpUrl  = "/api/v1/Identity/Auth/VerifyOtp";

    public const string DiscountsUrl  = "/api/v1/Discounts";
    public const string PackagesUrl   = "/api/v1/Packages";
    public const string ProductUrl    = "/api/v1/Product";
    public const string MarketingUrl  = "/api/v1/Marketing";

    // ── JWT Token ─────────────────────────────────────────────────────────────

    /// <summary>OTP درخواست می‌کند و JWT برمی‌گرداند</summary>
    public static async Task<string> GetTokenAsync(
        HttpClient client,
        string phone = AdminPhone)
    {
        // Step 1: RequestOtp
        var otpResp = await client.PostAsJsonAsync(RequestOtpUrl, new { PhoneNumber = phone });
        otpResp.EnsureSuccessStatusCode();

        // Step 2: VerifyOtp → JWT
        var loginResp = await client.PostAsJsonAsync(VerifyOtpUrl,
            new { PhoneNumber = phone, Code = OtpCode });
        loginResp.EnsureSuccessStatusCode();

        var body = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        var token = ExtractToken(body);

        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException(
                $"JWT token not found in response. Body: {body}");

        return token;
    }

    /// <summary>HttpClient را با Authorization header برمی‌گرداند</summary>
    public static async Task<HttpClient> AuthorizedAsync(
        HttpClient client, string phone = AdminPhone)
    {
        var token = await GetTokenAsync(client, phone);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // ── JSON helpers ───────────────────────────────────────────────────────────

    private static string? ExtractToken(JsonElement root)
    {
        // ApiResponse<LoginCommandResponse> → data.accessToken
        if (root.TryGetProperty("data", out var data) &&
            data.TryGetProperty("accessToken", out var t))
            return t.GetString();

        // fallback: مستقیم accessToken
        if (root.TryGetProperty("accessToken", out var t2))
            return t2.GetString();

        return null;
    }

    public static JsonElement? GetData(JsonElement root)
    {
        if (root.TryGetProperty("data", out var data)) return data;
        return null;
    }

    public static long GetDataId(JsonElement root)
    {
        var data = GetData(root);
        if (data.HasValue && data.Value.TryGetProperty("id", out var id))
            return id.GetInt64();
        if (root.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Number)
            return d.GetInt64();
        // اگر data عدد باشد (مثلاً ID مستقیم)
        if (root.TryGetProperty("data", out var n) && n.ValueKind == JsonValueKind.Number)
            return n.GetInt64();
        return 0;
    }
}
