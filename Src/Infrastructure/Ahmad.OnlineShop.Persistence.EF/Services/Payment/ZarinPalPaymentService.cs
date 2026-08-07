using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ahmad.OnlineShop.Application.Contract.Payment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Ahmad.OnlineShop.Persistence.EF.Services.Payment;

/// <summary>
/// پیاده‌سازی زرین‌پال با Circuit Breaker (Polly v8).
/// ترتیب pipeline: Timeout ← Retry ← CircuitBreaker (از درون به بیرون)
/// </summary>
public sealed class ZarinPalPaymentService : IZarinPalService
{
    private static readonly HttpClient _http = new();
    private readonly ZarinPalOptions _opts;
    private readonly ILogger<ZarinPalPaymentService>? _logger;
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ZarinPalPaymentService(IOptions<ZarinPalOptions> opts, ILogger<ZarinPalPaymentService>? logger = null)
    {
        _opts   = opts.Value;
        _logger = logger;

        _pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddTimeout(new TimeoutStrategyOptions { Timeout = TimeSpan.FromSeconds(15) })
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 1,
                Delay            = TimeSpan.FromSeconds(2),
                BackoffType      = DelayBackoffType.Constant,
                ShouldHandle     = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r => (int)r.StatusCode >= 500)
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                MinimumThroughput = 2,
                FailureRatio      = 0.5,
                SamplingDuration  = TimeSpan.FromSeconds(60),
                BreakDuration     = TimeSpan.FromSeconds(120),
                ShouldHandle      = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r => (int)r.StatusCode >= 500),
                OnOpened = args =>
                {
                    _logger?.LogWarning("Circuit زرین‌پال باز شد. مدت توقف: {Break}", args.BreakDuration);
                    return ValueTask.CompletedTask;
                },
                OnClosed = _ =>
                {
                    _logger?.LogInformation("Circuit زرین‌پال بسته شد — درگاه دوباره در دسترس است");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public async Task<ZarinPalRequestResult> RequestAsync(
        decimal amount, string description, string callbackUrl, CancellationToken token = default)
    {
        try
        {
            var body = new ZarinPalRequestBody(
                _opts.MerchantId, (long)amount, "IRT", description, callbackUrl);

            var response = await _pipeline.ExecuteAsync(async ct =>
            {
                var req = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{_opts.BaseUrl}/request.json")
                {
                    Content = JsonContent.Create(body)
                };
                return await _http.SendAsync(req, ct);
            }, token);

            if (!response.IsSuccessStatusCode)
                return new ZarinPalRequestResult(false, null, null, $"HTTP {(int)response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<ZarinPalApiResponse<ZarinPalRequestData>>(_json, token);

            if (result?.Data?.Code is not (100 or 101))
                return new ZarinPalRequestResult(false, null, null, result?.Data?.Message ?? "خطای درگاه پرداخت");

            var authority  = result.Data.Authority!;
            var redirectUrl = $"{_opts.StartPayUrl}/{authority}";

            _logger?.LogInformation("ZarinPal request موفق. Authority: {Authority}", authority);
            return new ZarinPalRequestResult(true, authority, redirectUrl);
        }
        catch (BrokenCircuitException ex)
        {
            _logger?.LogWarning(ex, "Circuit زرین‌پال باز است (request)");
            throw;   // کالر (handler) باید BrokenCircuitException را catch و fallback انجام دهد
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "خطا در ارتباط با زرین‌پال (request)");
            return new ZarinPalRequestResult(false, null, null, ex.Message);
        }
    }

    public async Task<ZarinPalVerifyResult> VerifyAsync(
        string authority, decimal amount, CancellationToken token = default)
    {
        try
        {
            var body = new ZarinPalVerifyBody(_opts.MerchantId, (long)amount, authority);

            var response = await _pipeline.ExecuteAsync(async ct =>
            {
                var req = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{_opts.BaseUrl}/verify.json")
                {
                    Content = JsonContent.Create(body)
                };
                return await _http.SendAsync(req, ct);
            }, token);

            if (!response.IsSuccessStatusCode)
                return new ZarinPalVerifyResult(false, null, null, $"HTTP {(int)response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<ZarinPalApiResponse<ZarinPalVerifyData>>(_json, token);

            if (result?.Data?.Code is not (100 or 101))
                return new ZarinPalVerifyResult(false, null, result?.Data?.Code, result?.Data?.Message ?? "تأیید ناموفق");

            _logger?.LogInformation("ZarinPal verify موفق. RefId: {RefId}", result.Data.RefId);
            return new ZarinPalVerifyResult(true, result.Data.RefId?.ToString(), result.Data.Code);
        }
        catch (BrokenCircuitException ex)
        {
            _logger?.LogWarning(ex, "Circuit زرین‌پال باز است (verify)");
            return new ZarinPalVerifyResult(false, null, null, "درگاه پرداخت موقتاً در دسترس نیست");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "خطا در ارتباط با زرین‌پال (verify)");
            return new ZarinPalVerifyResult(false, null, null, ex.Message);
        }
    }

    // ── Request/Response Models ────────────────────────────────────────────────

    private sealed record ZarinPalRequestBody(
        [property: JsonPropertyName("merchant_id")] string  MerchantId,
        [property: JsonPropertyName("amount")]      long    Amount,
        [property: JsonPropertyName("currency")]    string  Currency,
        [property: JsonPropertyName("description")] string  Description,
        [property: JsonPropertyName("callback_url")] string CallbackUrl);

    private sealed record ZarinPalVerifyBody(
        [property: JsonPropertyName("merchant_id")] string MerchantId,
        [property: JsonPropertyName("amount")]      long   Amount,
        [property: JsonPropertyName("authority")]   string Authority);

    private sealed class ZarinPalApiResponse<T>
    {
        [JsonPropertyName("data")]   public T?    Data   { get; set; }
        [JsonPropertyName("errors")] public object? Errors { get; set; }
    }

    private sealed class ZarinPalRequestData
    {
        [JsonPropertyName("code")]      public int?    Code      { get; set; }
        [JsonPropertyName("message")]   public string? Message   { get; set; }
        [JsonPropertyName("authority")] public string? Authority { get; set; }
    }

    private sealed class ZarinPalVerifyData
    {
        [JsonPropertyName("code")]    public int?    Code   { get; set; }
        [JsonPropertyName("message")] public string? Message { get; set; }
        [JsonPropertyName("ref_id")]  public long?   RefId  { get; set; }
    }
}
