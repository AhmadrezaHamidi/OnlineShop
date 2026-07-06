using Ahmad.OnlineShop.Persistence.EF;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Application.Tests.Fakes;

/// <summary>InMemory ApplicationDbContext برای تست‌ها</summary>
public static class FakeAppDb
{
    public static ApplicationDbContext Create()
    {
        var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(opts);
    }
}
