namespace Ahmad.OnlineShop.Persistence.EF.Services.Sms;

/// <summary>Mapper برای ساختن payload ارسال SMS از پارامترهای ساده</summary>
public static class SmsMapper
{
    /// <summary>تبدیل شماره موبایل و کد OTP به payload آماده ارسال</summary>
    public static SmsBulkRequest ToOtpRequest(
        string phoneNumber,
        string code,
        SmsOptions options)
        => new(
            LineNumber:   options.LineNumber,
            MessageText:  string.Format(options.OtpTemplate, code),
            Mobiles:      [phoneNumber],
            SendDateTime: null);

    /// <summary>تبدیل لیست شماره‌ها و متن دلخواه به payload</summary>
    public static SmsBulkRequest ToBulkRequest(
        List<string> mobiles,
        string       messageText,
        SmsOptions   options)
        => new(
            LineNumber:   options.LineNumber,
            MessageText:  messageText,
            Mobiles:      mobiles,
            SendDateTime: null);
}
