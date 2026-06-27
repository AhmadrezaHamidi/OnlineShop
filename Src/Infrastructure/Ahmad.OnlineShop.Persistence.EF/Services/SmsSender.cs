using Ahmad.OnlineShop.Persistence.EF.Services.Sms;
using Identity.Application.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Ahmad.OnlineShop.Persistence.EF.Services;

/// <summary>ارسال SMS از طریق sms.ir با استفاده از IOptions و مدل‌های typed</summary>
public sealed class SmsSender : ISmsService
{
    private readonly SmsOptions _options;

    public SmsSender(IOptions<SmsOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>ارسال کد OTP به شماره موبایل</summary>
    public async Task<bool> SendOtpAsync(
        string            phoneNumber,
        string            code,
        CancellationToken token = default)
    {
        var payload = SmsMapper.ToOtpRequest(phoneNumber, code, _options);
        return await SendAsync(payload, token);
    }

    /// <summary>ارسال پیام دلخواه به چند شماره</summary>
    public async Task<bool> SendBulkAsync(
        IReadOnlyList<string> phones,
        string                message,
        CancellationToken     token = default)
    {
        var payload = SmsMapper.ToBulkRequest(phones.ToList(), message, _options);
        return await SendAsync(payload, token);
    }

    // ─── Private ─────────────────────────────────────────────────────────────

    private async Task<bool> SendAsync(SmsBulkRequest payload, CancellationToken token)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);

        try
        {
            var response = await http.PostAsJsonAsync(_options.BaseUrl, payload, token);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
