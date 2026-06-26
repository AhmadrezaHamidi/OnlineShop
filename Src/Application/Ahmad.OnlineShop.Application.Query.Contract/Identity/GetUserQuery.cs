using AhmadBase.Application.Query;
using Identity.Application.Dtos;

namespace Identity.Application.Query.Queries;

public record GetUserQuery(long UserId) : IQuery<UserDto>;
