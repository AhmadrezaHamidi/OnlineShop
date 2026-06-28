namespace Ahmad.OnlineShop.Read.Dapper.Order;

/// <summary>
/// Dapper read repository برای سفارشات — از multi-mapping Dapper استفاده می‌کند
/// </summary>
public sealed class OrderReadRepository(IDbConnection connection)
{
    public async Task<GetOrderQueryResponse?> GetByIdAsync(long id, CancellationToken token = default)
    {
        const string sql = """
            SELECT
                o.Id, o.UserId, o.Status, o.TotalAmount, o.PaymentMethod,
                o.PlacedAt, o.CreatedAt,
                i.Id   AS ItemId,   i.ProductId, i.Quantity, i.UnitPrice, i.TotalPrice,
                p.Id   AS PayId,    p.Amount, p.Status AS PayStatus, p.Provider, p.PaidAt,
                p.IsSuccessful
            FROM [dbo].[Orders] o
            LEFT JOIN [dbo].[OrderItems]    i ON i.OrderId = o.Id
            LEFT JOIN [dbo].[Payments]      p ON p.OrderId = o.Id
            WHERE o.Id = @Id
            """;

        GetOrderQueryResponse? response = null;
        var items    = new Dictionary<long, GetOrderItemResponse>();
        var payments = new Dictionary<long, GetPaymentResponse>();

        await connection.QueryAsync<dynamic>(sql, (row) =>
        {
            if (response is null)
            {
                response = new GetOrderQueryResponse(
                    Id:            row.Id,
                    UserId:        row.UserId,
                    Status:        (OrderStatus)row.Status,
                    TotalAmount:   row.TotalAmount,
                    PaymentMethod: (PaymentMethod)row.PaymentMethod,
                    PlacedAt:      row.PlacedAt,
                    Items:         new List<GetOrderItemResponse>(),
                    Payments:      new List<GetPaymentResponse>());
            }

            if (row.ItemId != null && !items.ContainsKey((long)row.ItemId))
            {
                var item = new GetOrderItemResponse(
                    Id:         (long)row.ItemId,
                    ProductId:  row.ProductId,
                    Quantity:   row.Quantity,
                    UnitPrice:  row.UnitPrice,
                    TotalPrice: row.TotalPrice);
                items[(long)row.ItemId] = item;
            }

            if (row.PayId != null && !payments.ContainsKey((long)row.PayId))
            {
                var payment = new GetPaymentResponse(
                    Id:           (long)row.PayId,
                    Amount:       row.Amount,
                    Status:       (PaymentStatus)row.PayStatus,
                    Provider:     row.Provider,
                    PaidAt:       row.PaidAt,
                    IsSuccessful: row.IsSuccessful);
                payments[(long)row.PayId] = payment;
            }

            return response;
        }, new { Id = id });

        if (response is null) return null;

        return response with
        {
            Items    = items.Values.ToList(),
            Payments = payments.Values.ToList()
        };
    }

    public async Task<(List<GetOrderQueryResponse> Items, int Total)> GetListAsync(
        int page, int pageSize,
        long? userId, OrderStatus? status,
        CancellationToken token = default)
    {
        var where = new List<string>();
        var param = new DynamicParameters();

        if (userId.HasValue) { where.Add("UserId = @UserId"); param.Add("UserId", userId.Value); }
        if (status.HasValue) { where.Add("Status = @Status"); param.Add("Status", status.Value); }

        var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";
        var offset      = (page - 1) * pageSize;
        param.Add("Offset", offset); param.Add("PageSize", pageSize);

        var countSql = $"SELECT COUNT(*) FROM [dbo].[Orders] {whereClause}";
        var dataSql  = $"""
            SELECT Id, UserId, Status, TotalAmount, PaymentMethod, PlacedAt, CreatedAt
            FROM [dbo].[Orders]
            {whereClause}
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

        var total = await connection.QuerySingleAsync<int>(countSql, param);
        var rows  = await connection.QueryAsync<OrderRow>(dataSql, param);

        var items = rows.Select(r => new GetOrderQueryResponse(
            Id:            r.Id,
            UserId:        r.UserId,
            Status:        r.Status,
            TotalAmount:   r.TotalAmount,
            PaymentMethod: r.PaymentMethod,
            PlacedAt:      r.PlacedAt,
            Items:         new List<GetOrderItemResponse>(),
            Payments:      new List<GetPaymentResponse>()
        )).ToList();

        return (items, total);
    }

    private sealed class OrderRow
    {
        public long          Id            { get; set; }
        public long          UserId        { get; set; }
        public OrderStatus   Status        { get; set; }
        public decimal       TotalAmount   { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime      PlacedAt      { get; set; }
        public DateTime      CreatedAt     { get; set; }
    }
}
