using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Application.Query;

namespace BackOffice.Application.Query.Queries;

public record GetAdminUserQuery(long AdminId) : IQuery<GetAdminUserQueryResponse>;

public sealed record GetAdminUserQueryResponse(
    long        Id,
    string      FullName,
    string      Email,
    AdminRole   Role,
    AdminStatus Status,
    DateTime    CreatedAt);
