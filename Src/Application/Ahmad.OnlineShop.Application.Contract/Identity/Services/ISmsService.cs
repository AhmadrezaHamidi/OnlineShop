namespace Identity.Application.Services;

public interface ISmsService
{
    Task<bool> SendOtpAsync(string phoneNumber, string code, CancellationToken token = default);
}
