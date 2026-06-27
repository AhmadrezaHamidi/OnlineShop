using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(long userId, CancellationToken ct = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task DeleteAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task DeleteAllForUserAsync(long userId, CancellationToken ct = default);
    Task<long> GetNextIdAsync();
}
