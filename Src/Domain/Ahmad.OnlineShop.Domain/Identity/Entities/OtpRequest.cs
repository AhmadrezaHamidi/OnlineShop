using Identity.Domain.Exceptions;

namespace Identity.Domain.Entities;

/// <summary>درخواست رمز یکبار مصرف (OTP) برای احراز هویت با شماره موبایل</summary>
public sealed class OtpRequest
{
    public long     Id          { get; private set; }
    public string   PhoneNumber { get; private set; } = string.Empty;
    public string   Code        { get; private set; } = string.Empty;
    public DateTime ExpiresAt   { get; private set; }
    public bool     IsUsed      { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    private OtpRequest() { }

    public OtpRequest(long id, string phoneNumber, string code, int expiryMinutes = 5)
    {
        Id          = id;
        PhoneNumber = phoneNumber;
        Code        = code;
        ExpiresAt   = DateTime.UtcNow.AddMinutes(expiryMinutes);
        IsUsed      = false;
    }

    public void MarkUsed()
    {
        if (IsUsed || IsExpired)
            throw new InvalidOtpException();

        IsUsed = true;
    }

    public void Verify(string code)
    {
        if (IsUsed || IsExpired || Code != code)
            throw new InvalidOtpException();

        IsUsed = true;
    }
}
