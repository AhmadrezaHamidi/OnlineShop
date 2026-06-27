namespace Ahmad.OnlineShop.Persistence.EF.Services.Payment;

public sealed class ZarinPalOptions
{
    public const string Section = "ZarinPal";

    public string MerchantId  { get; set; } = string.Empty;
    public bool   Sandbox     { get; set; } = true;

    public string BaseUrl     => Sandbox
        ? "https://sandbox.zarinpal.com/pg/v4/payment"
        : "https://api.zarinpal.com/pg/v4/payment";

    public string StartPayUrl => Sandbox
        ? "https://sandbox.zarinpal.com/pg/StartPay"
        : "https://www.zarinpal.com/pg/StartPay";
}
