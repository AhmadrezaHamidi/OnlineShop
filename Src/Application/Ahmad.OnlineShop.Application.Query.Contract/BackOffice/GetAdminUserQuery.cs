using AhmadBase.Application.Query;

namespace BackOffice.Application.Query.Queries;

public record GetAdminUserQuery(long AdminId) : IQuery<AdminUserDto>;
