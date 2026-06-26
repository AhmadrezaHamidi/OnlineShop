using AhmadBase.Application.Query;
using Identity.Application.Dtos;

namespace Identity.Application.Query.Queries;

public record GetRolesQuery() : IQuery<IReadOnlyList<RoleDto>>;
