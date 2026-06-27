namespace Ahmad.OnlineShop.Persistence.EF.Services.Sms;

/// <summary>تنظیمات SMS — از appsettings.json بارگذاری می‌شود</summary>
public sealed class SmsOptions
{
    public const string Section = "Sms";

    public string ApiKey     { get; set; } = string.Empty;
    public long   LineNumber { get; set; }
    public string BaseUrl    { get; set; } = "https://api.sms.ir/v1/send/bulk";
    public string OtpTemplate { get; set; } = "کد تأیید شما: {0}\nاین کد تا ۵ دقیقه معتبر است.";
}
