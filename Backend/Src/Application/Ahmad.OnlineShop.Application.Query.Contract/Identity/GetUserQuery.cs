using AhmadBase.Application.Query;
using Identity.Domain.Enums;

namespace Identity.Application.Query.Queries;

public record GetUserQuery(long UserId) : IQuery<GetUserQueryResponse>;

public sealed record GetUserQueryResponse(
    long                      Id,
    string                    FullName,
    string                    Email,
    string?                   PhoneNumber,
    UserStatus                Status,
    DateTime                  CreatedAt,
    IReadOnlyCollection<long> RoleIds);
