namespace Ahmad.OnlineShop.Read.Dapper.BackOffice;

/// <summary>
/// پیاده‌سازی Dapper برای خواندن اطلاعات ادمین‌ها در BackOffice
/// مستقیماً از view یا جداول DB می‌خواند — بدون EF overhead
/// </summary>
public sealed class AdminUserReadRepository(IDbConnection connection) : IAdminUserReadRepository
{
    public async Task<GetAdminUserQueryResponse?> GetByIdAsync(
        long adminId, CancellationToken token = default)
    {
        const string sql = """
            SELECT
                Id, FullName, Email,
                Role, Status, CreatedAt
            FROM [dbo].[AdminUsers]
            WHERE Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<GetAdminUserQueryResponse>(
            sql, new { Id = adminId });
    }

    public async Task<(List<GetAdminUserQueryResponse> Items, int Total)> GetListAsync(
        int page, int pageSize,
        AdminStatus? status,
        AdminRole?   role,
        CancellationToken token = default)
    {
        var where = new List<string>();
        var param = new DynamicParameters();

        if (status.HasValue) { where.Add("Status = @Status"); param.Add("Status", status.Value); }
        if (role.HasValue)   { where.Add("Role = @Role");     param.Add("Role", role.Value); }

        var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";
        var offset      = (page - 1) * pageSize;

        param.Add("Offset",   offset);
        param.Add("PageSize", pageSize);

        var countSql = $"SELECT COUNT(*) FROM [dbo].[AdminUsers] {whereClause}";
        var dataSql  = $"""
            SELECT Id, FullName, Email, Role, Status, CreatedAt
            FROM [dbo].[AdminUsers]
            {whereClause}
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var total = await connection.QuerySingleAsync<int>(countSql, param);
        var items = (await connection.QueryAsync<GetAdminUserQueryResponse>(dataSql, param)).ToList();

        return (items, total);
    }

    public async Task<List<GetReportQueryResponse>> GetReportsAsync(
        long adminId, int page, int pageSize, CancellationToken token = default)
    {
        var offset = (page - 1) * pageSize;
        const string sql = """
            SELECT Id, AdminUserId, Type, Status, FilePath, GeneratedAt, FailReason
            FROM [dbo].[Reports]
            WHERE AdminUserId = @AdminId
            ORDER BY GeneratedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var result = await connection.QueryAsync<GetReportQueryResponse>(
            sql, new { AdminId = adminId, Offset = offset, PageSize = pageSize });

        return result.ToList();
    }

    public async Task<List<GetAuditLogQueryResponse>> GetAuditLogsAsync(
        long adminId, int page, int pageSize, CancellationToken token = default)
    {
        var offset = (page - 1) * pageSize;
        const string sql = """
            SELECT Id, AdminUserId, Action, EntityType, EntityId, OldValue, NewValue, CreatedAt
            FROM [dbo].[AuditLogs]
            WHERE AdminUserId = @AdminId
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var result = await connection.QueryAsync<GetAuditLogQueryResponse>(
            sql, new { AdminId = adminId, Offset = offset, PageSize = pageSize });

        return result.ToList();
    }
}
