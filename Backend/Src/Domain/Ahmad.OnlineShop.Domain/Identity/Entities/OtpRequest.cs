using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

/// <summary>
/// درخواست رمز یکبار مصرف (OTP) برای احراز هویت با شماره موبایل.
///
/// نقشه راه: در نسخه 5.1.0 AhmadBase.Doamin این class به NuGet منتقل می‌شود.
/// تبدیل: این class از AhmadBase.Doamin.OtpRequest ارث می‌برد،
/// و فقط throw منطق را اضافه می‌کند (NuGet فقط bool برمی‌گرداند).
/// </summary>
public sealed class OtpRequest
{
    public long Id { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;

    private OtpRequest() { }

    public OtpRequest(long id, string phoneNumber, string code, int expiryMinutes = 5)
    {
        Id = id;
        PhoneNumber = phoneNumber;
        Code = code;
        ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
        IsUsed = false;
    }

    /// <summary>
    /// تأیید کد OTP — اگر نامعتبر یا منقضی باشد، InvalidOtpException می‌اندازد.
    /// بعد از تأیید موفق IsUsed = true می‌شود (جلوگیری از Replay Attack).
    /// </summary>
    public void Verify(string inputCode)
    {
        if (!IsValid || Code != inputCode)
            throw new InvalidOtpException();

        IsUsed = true;
    }
}
