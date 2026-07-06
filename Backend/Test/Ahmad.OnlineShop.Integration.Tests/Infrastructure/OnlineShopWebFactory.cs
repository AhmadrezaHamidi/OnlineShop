using Ahmad.OnlineShop.Persistence.EF.Dev;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahmad.OnlineShop.Integration.Tests.Infrastructure;

/// <summary>
/// WebApplicationFactory برای Integration Tests
/// سرور واقعی در حافظه بالا می‌آید و به LocalDB احمد متصل می‌شود
/// Database: Ahmad.OnlineShopDb_IT
/// </summary>
public sealed class OnlineShopWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string TestDb =
        "Server=(localdb)\\MSSQLLocalDB;Database=Ahmad.OnlineShopDb_IT;" +
        "Integrated Security=True;TrustServerCertificate=True;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // محیط Testing — جلوگیری از اجرای DevSeedData در Program.cs
        builder.UseEnvironment("Testing");

        // اضافه کردن appsettings.Testing.json
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false);
        });

        // Redis را با InMemory جایگزین می‌کنیم تا نیازی به Redis نباشد
        builder.ConfigureTestServices(services =>
        {
            var redis = services.FirstOrDefault(d =>
                d.ServiceType.FullName?.Contains("IDistributedCache") == true ||
                d.ServiceType.FullName?.Contains("RedisCache") == true);

            if (redis is not null) services.Remove(redis);
            services.AddDistributedMemoryCache();
        });
    }

    // ── IAsyncLifetime ─────────────────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        // Migration اجرا + Seed داده‌های اولیه روی TestDB
        await DevSeedData.SeedAsync(TestDb);
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}

/// <summary>
/// Collection مشترک — همه تست‌های Integration از یک Factory استفاده می‌کنند
/// تا DB یک‌بار migrate شود و سرور یک‌بار بالا بیاید
/// </summary>
[CollectionDefinition("Integration")]
public sealed class IntegrationCollection : ICollectionFixture<OnlineShopWebFactory> { }
