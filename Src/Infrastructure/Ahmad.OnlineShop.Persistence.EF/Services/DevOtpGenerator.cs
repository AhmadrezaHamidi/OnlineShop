namespace Ahmad.OnlineShop.Persistence.EF.Services;

/// <summary>
/// در Development همیشه کد ثابت 00000 برمی‌گرداند
/// در Production از کد تصادفی واقعی استفاده می‌شود
/// </summary>
public static class OtpCodeGenerator
{
    private static bool _isDevelopment;

    public static void ConfigureForDevelopment() => _isDevelopment = true;

    public static string Generate() =>
        _isDevelopment ? "00000" : new Random().Next(10000, 99999).ToString();
}
