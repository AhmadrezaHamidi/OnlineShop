namespace Ahmad.OnlineShop.Read.Dapper.Products;

/// <summary>
/// Dapper read repository برای محصولات
/// از view V_Products استفاده می‌کند که موجودی و تصاویر را هم join می‌زند
/// </summary>
public sealed class ProductReadRepository(IDbConnection connection)
{
    public async Task<GetProductQueryResponse?> GetByIdAsync(long id, CancellationToken token = default)
    {
        // ── محصول اصلی ─────────────────────────────────────────────────────────
        const string productSql = """
            SELECT
                p.Id, p.SellerId, p.CategoryId, p.Name, p.Description,
                p.Price, p.Status, p.CreationTime,
                inv.ProductId   AS Inv_ProductId,
                inv.Quantity    AS Inv_Quantity,
                inv.ReservedQuantity AS Inv_ReservedQuantity,
                inv.AvailableQuantity AS Inv_AvailableQuantity,
                inv.IsLowStock  AS Inv_IsLowStock,
                inv.IsOutOfStock AS Inv_IsOutOfStock
            FROM [dbo].[Products] p
            LEFT JOIN [dbo].[Inventories] inv ON inv.ProductId = p.Id
            WHERE p.Id = @Id
            """;

        // ── تصاویر ─────────────────────────────────────────────────────────────
        const string imagesSql = """
            SELECT Id, Url, Type, SortOrder, UploadedAt
            FROM [dbo].[ProductImages]
            WHERE ProductId = @Id
            ORDER BY SortOrder
            """;

        var product = await connection.QueryFirstOrDefaultAsync<ProductWithInvRow>(productSql, new { Id = id });
        if (product is null) return null;

        var images = (await connection.QueryAsync<GetProductImageResponse>(imagesSql, new { Id = id })).ToList();

        return MapToResponse(product, images);
    }

    public async Task<(List<GetProductQueryResponse> Items, int Total)> GetListAsync(
        int page, int pageSize, string? search, long? categoryId,
        ProductStatus? status, CancellationToken token = default)
    {
        var where = new List<string>();
        var param = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            where.Add("p.Name LIKE @Search");
            param.Add("Search", $"%{search}%");
        }
        if (categoryId.HasValue) { where.Add("p.CategoryId = @CategoryId"); param.Add("CategoryId", categoryId.Value); }
        if (status.HasValue)     { where.Add("p.Status = @Status");         param.Add("Status", status.Value); }

        var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";
        var offset      = (page - 1) * pageSize;
        param.Add("Offset", offset); param.Add("PageSize", pageSize);

        var countSql = $"SELECT COUNT(*) FROM [dbo].[Products] p {whereClause}";
        var dataSql  = $"""
            SELECT
                p.Id, p.SellerId, p.CategoryId, p.Name, p.Description,
                p.Price, p.Status, p.CreationTime,
                inv.ProductId AS Inv_ProductId,
                inv.Quantity AS Inv_Quantity,
                inv.ReservedQuantity AS Inv_ReservedQuantity,
                inv.AvailableQuantity AS Inv_AvailableQuantity,
                inv.IsLowStock AS Inv_IsLowStock,
                inv.IsOutOfStock AS Inv_IsOutOfStock
            FROM [dbo].[Products] p
            LEFT JOIN [dbo].[Inventories] inv ON inv.ProductId = p.Id
            {whereClause}
            ORDER BY p.CreationTime DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var total = await connection.QuerySingleAsync<int>(countSql, param);
        var rows  = (await connection.QueryAsync<ProductWithInvRow>(dataSql, param)).ToList();
        var items = rows.Select(r => MapToResponse(r, new List<GetProductImageResponse>())).ToList();

        return (items, total);
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static GetProductQueryResponse MapToResponse(ProductWithInvRow r, List<GetProductImageResponse> images)
        => new(
            Id:          r.Id,
            CategoryId:  r.CategoryId,
            Name:        r.Name,
            Description: r.Description,
            Price:       r.Price,
            Status:      r.Status,
            CreatedAt:   r.CreationTime,
            Inventory:   new GetProductInventoryResponse(
                ProductId:         r.Inv_ProductId,
                Quantity:          r.Inv_Quantity,
                ReservedQuantity:  r.Inv_ReservedQuantity,
                AvailableQuantity: r.Inv_AvailableQuantity,
                IsLowStock:        r.Inv_IsLowStock,
                IsOutOfStock:      r.Inv_IsOutOfStock),
            Images:      images);

    // ── Flat row model ────────────────────────────────────────────────────────
    private sealed class ProductWithInvRow
    {
        public long            Id                 { get; set; }
        public long            SellerId           { get; set; }
        public long            CategoryId         { get; set; }
        public string          Name               { get; set; } = "";
        public string?         Description        { get; set; }
        public decimal         Price              { get; set; }
        public ProductStatus   Status             { get; set; }
        public DateTimeOffset  CreationTime       { get; set; }
        public long            Inv_ProductId      { get; set; }
        public int             Inv_Quantity       { get; set; }
        public int             Inv_ReservedQuantity  { get; set; }
        public int             Inv_AvailableQuantity { get; set; }
        public bool            Inv_IsLowStock     { get; set; }
        public bool            Inv_IsOutOfStock   { get; set; }
    }
}
