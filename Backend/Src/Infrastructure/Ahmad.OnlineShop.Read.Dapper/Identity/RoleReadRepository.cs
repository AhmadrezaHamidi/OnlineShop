namespace Ahmad.OnlineShop.Read.Dapper.Identity;

public sealed class RoleReadRepository(IDbConnection connection) : IRoleReadRepository
{
    public async Task<IReadOnlyList<GetRoleQueryResponse>> GetAllAsync(CancellationToken token = default)
    {
        const string sql = "SELECT Id, Name FROM [dbo].[IdentityRoles] ORDER BY Name";

        var result = await connection.QueryAsync<GetRoleQueryResponse>(sql);
        return result.ToList().AsReadOnly();
    }
}
