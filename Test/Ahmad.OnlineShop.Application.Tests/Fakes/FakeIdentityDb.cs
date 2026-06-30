using Ahmad.OnlineShop.Persistence.EF;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Application.Tests.Fakes;

/// <summary>InMemory IdentityAppDbContext برای تست‌ها</summary>
public static class FakeIdentityDb
{
    public static IdentityAppDbContext Create()
    {
        var opts = new DbContextOptionsBuilder<IdentityAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new IdentityAppDbContext(opts);
    }
}
