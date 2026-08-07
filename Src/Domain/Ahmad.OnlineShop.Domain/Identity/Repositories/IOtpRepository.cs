using Identity.Domain.Entities;
using OtpRequest = Identity.Domain.Entities.OtpRequest;

namespace Identity.Domain.Repositories;

public interface IOtpRepository
{
    Task<OtpRequest?> GetLatestByPhoneAsync(string phoneNumber, CancellationToken token = default);
    Task AddAsync(OtpRequest otp, CancellationToken token = default);
    Task UpdateAsync(OtpRequest otp, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
