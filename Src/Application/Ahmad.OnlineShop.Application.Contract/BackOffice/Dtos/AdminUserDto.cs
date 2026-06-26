using BackOffice.Domain.Enums;

namespace BackOffice.Application.Dtos;

public sealed record AdminUserDto(
    long        Id,
    string      FullName,
    string      Email,
    AdminRole   Role,
    AdminStatus Status,
    DateTime    CreatedAt
);
