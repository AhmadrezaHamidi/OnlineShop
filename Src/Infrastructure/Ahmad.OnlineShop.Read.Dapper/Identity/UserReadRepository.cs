namespace Ahmad.OnlineShop.Read.Dapper.Identity;

/// <summary>
/// Dapper read repository برای کاربران Identity — سرعت بالا برای listing
/// </summary>
public sealed class UserReadRepository(IDbConnection connection) : IUserReadRepository
{
    public async Task<GetUserQueryResponse?> GetByIdAsync(long userId, CancellationToken token = default)
    {
        const string sql = """
            SELECT Id, PhoneNumber, FullName, Status, CreatedAt, RoleIds
            FROM [dbo].[IdentityUsers]
            WHERE Id = @UserId
            """;

        var row = await connection.QueryFirstOrDefaultAsync<UserRow>(sql, new { UserId = userId });
        return row?.ToResponse();
    }

    public async Task<(IReadOnlyList<GetUserQueryResponse> Items, int Total)> GetListAsync(
        int page, int pageSize,
        string?     search,
        UserStatus? status,
        CancellationToken token = default)
    {
        var where = new List<string>();
        var param = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where.Add("(PhoneNumber LIKE @Search OR FullName LIKE @Search)");
            param.Add("Search", $"%{search}%");
        }

        if (status.HasValue) { where.Add("Status = @Status"); param.Add("Status", status.Value); }

        var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";
        var offset      = (page - 1) * pageSize;

        param.Add("Offset",   offset);
        param.Add("PageSize", pageSize);

        var countSql = $"SELECT COUNT(*) FROM [dbo].[IdentityUsers] {whereClause}";
        var dataSql  = $"""
            SELECT Id, PhoneNumber, FullName, Status, CreatedAt, RoleIds
            FROM [dbo].[IdentityUsers]
            {whereClause}
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var total = await connection.QuerySingleAsync<int>(countSql, param);
        var items = (await connection.QueryAsync<UserRow>(dataSql, param))
                        .Select(r => r.ToResponse())
                        .ToList()
                        .AsReadOnly();

        return (items, total);
    }

    // ── Row Mapping ───────────────────────────────────────────────────────────

    private sealed class UserRow
    {
        public long      Id          { get; set; }
        public string?   PhoneNumber { get; set; }
        public string?   FullName    { get; set; }
        public int       Status      { get; set; }
        public DateTime  CreatedAt   { get; set; }
        public string?   RoleIds     { get; set; }

        public GetUserQueryResponse ToResponse() => new(
            Id:          Id,
            FullName:    FullName ?? string.Empty,
            Email:       PhoneNumber ?? string.Empty,
            PhoneNumber: PhoneNumber,
            Status:      (UserStatus)Status,
            CreatedAt:   CreatedAt,
            RoleIds:     string.IsNullOrEmpty(RoleIds)
                            ? Array.Empty<long>()
                            : RoleIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray());
    }
}
