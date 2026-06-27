using AhmadBase.Application.Query;

namespace Identity.Application.Query.Queries;

public record GetRolesQuery() : IQuery<IReadOnlyList<GetRoleQueryResponse>>;

public sealed record GetRoleQueryResponse(
    long   Id,
    string Name);
