using Ahmad.OnlineShop.Application.Contract.Payment;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Ahmad.OnlineShop.Persistence.EF.Services.Payment;

/// <summary>
/// پیاده‌سازی درگاه زرین‌پال
/// API Reference: https://docs.zarinpal.com/paymentGateway/
/// Flow: Request → Redirect کاربر → Callback → Verify
/// </summary>
public sealed class ZarinPalService : IZarinPalService
{
    private readonly ZarinPalOptions _options;

    public ZarinPalService(IOptions<ZarinPalOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// مرحله ۱: درخواست پرداخت از زرین‌پال
    /// خروجی: Authority و لینک redirect به درگاه
    /// </summary>
    public async Task<ZarinPalRequestResult> RequestAsync(
        decimal           amount,
        string            description,
        string            callbackUrl,
        CancellationToken token = default)
    {
        var body = new ZarinPalRequestBody(
            MerchantId:  _options.MerchantId,
            Amount:      (long)amount,
            Description: description,
            CallbackUrl: callbackUrl);

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Accept", "application/json");

        try
        {
            var response = await http.PostAsJsonAsync(
                $"{_options.BaseUrl}/request.json", body, token);

            var result = await response.Content
                .ReadFromJsonAsync<ZarinPalApiResponse>(cancellationToken: token);

            if (result?.Data?.Code == 100 && result.Data.Authority is not null)
            {
                var redirectUrl = $"{_options.StartPayUrl}/{result.Data.Authority}";
                return new ZarinPalRequestResult(
                    Success:     true,
                    Authority:   result.Data.Authority,
                    RedirectUrl: redirectUrl);
            }

            var errorMsg = result?.Errors?.Message ?? "خطای ناشناخته";
            return new ZarinPalRequestResult(
                Success:  false,
                Authority: null,
                RedirectUrl: null,
                Error:    errorMsg);
        }
        catch (Exception ex)
        {
            return new ZarinPalRequestResult(
                Success: false, Authority: null, RedirectUrl: null,
                Error:   $"خطای ارتباط با زرین‌پال: {ex.Message}");
        }
    }

    /// <summary>
    /// مرحله ۲: تأیید پرداخت بعد از بازگشت کاربر از درگاه
    /// Status=OK در callback → Verify → دریافت کد تراکنش
    /// </summary>
    public async Task<ZarinPalVerifyResult> VerifyAsync(
        string            authority,
        decimal           amount,
        CancellationToken token = default)
    {
        var body = new ZarinPalVerifyBody(
            MerchantId: _options.MerchantId,
            Amount:     (long)amount,
            Authority:  authority);

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Accept", "application/json");

        try
        {
            var response = await http.PostAsJsonAsync(
                $"{_options.BaseUrl}/verify.json", body, token);

            var result = await response.Content
                .ReadFromJsonAsync<ZarinPalApiResponse>(cancellationToken: token);

            if (result?.Data?.Code is 100 or 101)
            {
                return new ZarinPalVerifyResult(
                    Success:         true,
                    TransactionCode: result.Data.RefId?.ToString(),
                    StatusCode:      result.Data.Code);
            }

            return new ZarinPalVerifyResult(
                Success:     false,
                TransactionCode: null,
                StatusCode:  result?.Data?.Code,
                Error:       result?.Errors?.Message ?? "تأیید ناموفق");
        }
        catch (Exception ex)
        {
            return new ZarinPalVerifyResult(
                Success: false, TransactionCode: null, StatusCode: null,
                Error:   $"خطای ارتباط: {ex.Message}");
        }
    }

    // ── Models ────────────────────────────────────────────────────────────

    private sealed record ZarinPalRequestBody(
        [property: JsonPropertyName("merchant_id")]  string MerchantId,
        [property: JsonPropertyName("amount")]       long   Amount,
        [property: JsonPropertyName("description")]  string Description,
        [property: JsonPropertyName("callback_url")] string CallbackUrl);

    private sealed record ZarinPalVerifyBody(
        [property: JsonPropertyName("merchant_id")] string MerchantId,
        [property: JsonPropertyName("amount")]      long   Amount,
        [property: JsonPropertyName("authority")]   string Authority);

    private sealed record ZarinPalApiResponse(
        [property: JsonPropertyName("data")]   ZarinPalData?  Data,
        [property: JsonPropertyName("errors")] ZarinPalError? Errors);

    private sealed record ZarinPalData(
        [property: JsonPropertyName("code")]      int?    Code,
        [property: JsonPropertyName("authority")] string? Authority,
        [property: JsonPropertyName("ref_id")]    long?   RefId);

    private sealed record ZarinPalError(
        [property: JsonPropertyName("message")] string? Message);
}
