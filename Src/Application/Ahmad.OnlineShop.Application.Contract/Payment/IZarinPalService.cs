namespace Ahmad.OnlineShop.Application.Contract.Payment;

/// <summary>سرویس درگاه پرداخت زرین‌پال</summary>
public interface IZarinPalService
{
    /// <summary>
    /// درخواست پرداخت — Authority و لینک redirect برمی‌گرداند
    /// </summary>
    Task<ZarinPalRequestResult> RequestAsync(
        decimal           amount,
        string            description,
        string            callbackUrl,
        CancellationToken token = default);

    /// <summary>
    /// تأیید پرداخت — بعد از بازگشت کاربر از درگاه
    /// </summary>
    Task<ZarinPalVerifyResult> VerifyAsync(
        string            authority,
        decimal           amount,
        CancellationToken token = default);
}

public sealed record ZarinPalRequestResult(
    bool    Success,
    string? Authority,
    string? RedirectUrl,
    string? Error = null);

public sealed record ZarinPalVerifyResult(
    bool    Success,
    string? TransactionCode,
    int?    StatusCode,
    string? Error = null);
