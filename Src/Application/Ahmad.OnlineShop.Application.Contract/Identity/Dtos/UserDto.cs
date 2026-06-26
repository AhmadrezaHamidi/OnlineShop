using Identity.Domain.Enums;

namespace Identity.Application.Dtos;

public sealed record UserDto(
    long       Id,
    string     FullName,
    string     Email,
    string?    PhoneNumber,
    UserStatus Status,
    DateTime   CreatedAt,
    IReadOnlyCollection<long> RoleIds
);
