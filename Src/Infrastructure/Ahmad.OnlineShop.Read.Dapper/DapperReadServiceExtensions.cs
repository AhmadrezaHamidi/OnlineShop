using Ahmad.OnlineShop.Read.Dapper.BackOffice;
using Ahmad.OnlineShop.Read.Dapper.Identity;
using Ahmad.OnlineShop.Read.Dapper.Order;
using Ahmad.OnlineShop.Read.Dapper.Products;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahmad.OnlineShop.Read.Dapper;

/// <summary>
/// ثبت همه Dapper Read Repository ها در DI
/// فراخوانی: services.AddDapperRead(configuration)
/// </summary>
public static class DapperReadServiceExtensions
{
    public static IServiceCollection AddDapperRead(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection رشته اتصال پیدا نشد.");

        // IDbConnection به صورت Scoped — هر request یک connection جدید
        services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

        // BackOffice
        services.AddScoped<IAdminUserReadRepository, AdminUserReadRepository>();

        // Identity
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IRoleReadRepository, RoleReadRepository>();

        // Products & Order (اگه query handler ها migrate شدن)
        services.AddScoped<ProductReadRepository>();
        services.AddScoped<OrderReadRepository>();

        return services;
    }
}
