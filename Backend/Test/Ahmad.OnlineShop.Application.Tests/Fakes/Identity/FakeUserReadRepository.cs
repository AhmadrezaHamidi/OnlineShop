/// <summary>Fake پیاده‌سازی IUserReadRepository برای تست MarketingHandlers</summary>
using Identity.Application.Query.Contracts;
using Identity.Application.Query.Queries;
using Identity.Domain.Enums;

namespace Ahmad.OnlineShop.Application.Tests.Fakes.Identity;

public class FakeUserReadRepository : IUserReadRepository
{
    private readonly List<string> _phones;

    public FakeUserReadRepository(IEnumerable<string>? phones = null)
        => _phones = phones?.ToList() ?? [];

    public Task<GetUserQueryResponse?> GetByIdAsync(long userId, CancellationToken token = default)
        => Task.FromResult<GetUserQueryResponse?>(null);

    public Task<(IReadOnlyList<GetUserQueryResponse> Items, int Total)> GetListAsync(
        int page, int pageSize, string? search, UserStatus? status, CancellationToken token = default)
        => Task.FromResult<(IReadOnlyList<GetUserQueryResponse>, int)>(([], 0));

    public Task<IReadOnlyList<string>> GetAllCustomerPhonesAsync(CancellationToken token = default)
        => Task.FromResult<IReadOnlyList<string>>(_phones);
}
