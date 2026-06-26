using Identity.Application.Dtos;
using Identity.Domain.Enums;

namespace Identity.Application.Query.Contracts;

/// <summary>
/// Read-side (query) repository for users.
/// Implemented in the Infrastructure/Read layer (e.g. Dapper).
/// </summary>
public interface IUserReadRepository
{
    Task<UserDto?> GetByIdAsync(long userId, CancellationToken token = default);

    Task<(IReadOnlyList<UserDto> Items, int Total)> GetListAsync(
        int         page,
        int         pageSize,
        string?     search,
        UserStatus? status,
        CancellationToken token = default);
}
