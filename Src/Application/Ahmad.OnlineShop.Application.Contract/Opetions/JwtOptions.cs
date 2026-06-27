namespace Ahmad.OnlineShop.Application.Contract.Opetions;

public class JWTOptions
{
    public string ValidIssuer          { get; set; } = string.Empty;
    public string ValidAudience        { get; set; } = string.Empty;
    public string Secret               { get; set; } = string.Empty;
    public int    TokenExpireMinutes   { get; set; } = 60;
    public int    RefreshExpireMinutes { get; set; } = 10080;
}
