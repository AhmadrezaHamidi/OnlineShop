using Identity.Application.Query.Queries;
using Identity.Domain.Enums;

namespace Identity.Application.Query.Contracts;

public interface IUserReadRepository
{
    Task<GetUserQueryResponse?> GetByIdAsync(long userId, CancellationToken token = default);

    Task<(IReadOnlyList<GetUserQueryResponse> Items, int Total)> GetListAsync(
        int page,
        int pageSize,
        string? search,
        UserStatus? status,
        CancellationToken token = default);

    Task<IReadOnlyList<string>> GetAllCustomerPhonesAsync(CancellationToken token = default);
}
