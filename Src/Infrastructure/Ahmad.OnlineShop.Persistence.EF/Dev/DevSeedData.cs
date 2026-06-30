using Ahmad.OnlineShop.Persistence.EF;
using Identity.Domain.Entities;
using Identity.Domain.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Args;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Dev;

/// <summary>
/// Seed داده‌های اولیه برای محیط Development — مستقل از DI container
/// فقط اگر جداول خالی باشن اجرا می‌شه. در Program.cs قبل از WebBuilder صدا زده می‌شود.
/// </summary>
public static class DevSeedData
{
    public static async Task SeedAsync(string connectionString)
    {
        var appOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString).Options;
        var identOptions = new DbContextOptionsBuilder<IdentityAppDbContext>()
            .UseSqlServer(connectionString).Options;

        await using var appDb   = new ApplicationDbContext(appOptions);
        await using var identDb = new IdentityAppDbContext(identOptions);

        Console.WriteLine("🌱 شروع Seed داده‌های Development ...");

        await appDb.Database.MigrateAsync();
        await identDb.Database.MigrateAsync();

        await SeedRolesAsync(identDb);
        await SeedUsersAsync(identDb);
        await SeedCategoryAsync(appDb);
        await SeedCreditLimitsAsync(appDb);

        Console.WriteLine("✅ Seed کامل شد");
    }

    private static async Task SeedRolesAsync(IdentityAppDbContext db)
    {
        if (await db.Roles.AnyAsync()) return;

        var roles = new[]
        {
            new Role(1, "Admin"),
            new Role(2, "Seller"),
            new Role(3, "Customer"),
        };

        await db.Roles.AddRangeAsync(roles);
        await db.SaveChangesAsync();
        Console.WriteLine("  ➕ نقش‌ها Seed شدند: Admin / Seller / Customer");
    }

    private static async Task SeedUsersAsync(IdentityAppDbContext db)
    {
        if (await db.IdentityUsers.AnyAsync()) return;

        var admin = User.Create(100, "09000000001");
        admin.UpdateProfile("Ahmad Admin");
        admin.AssignRole(1);
        await db.IdentityUsers.AddAsync(admin);

        var seller = User.Create(101, "09000000002");
        seller.UpdateProfile("Ahmad Seller");
        seller.AssignRole(2);
        await db.IdentityUsers.AddAsync(seller);

        var customer = User.Create(102, "09000000003");
        customer.UpdateProfile("Ahmad Customer");
        customer.AssignRole(3);
        await db.IdentityUsers.AddAsync(customer);

        await db.SaveChangesAsync();

        Console.WriteLine("  ➕ کاربران تست Seed شدند:");
        Console.WriteLine("     Admin    → 09000000001 (Id=100)");
        Console.WriteLine("     Seller   → 09000000002 (Id=101)");
        Console.WriteLine("     Customer → 09000000003 (Id=102)");
        Console.WriteLine("     OTP همه: 00000");
    }

    private static async Task SeedCategoryAsync(ApplicationDbContext db)
    {
        if (await db.Categories.AnyAsync()) return;

        var category = new Ahmad.OnlineShop.Domain.Products.Category(1, "لوازم جانبی", null);
        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync();
        Console.WriteLine("  ➕ دسته‌بندی پیش‌فرض Seed شد: لوازم جانبی (Id=1)");
    }

    private static async Task SeedCreditLimitsAsync(ApplicationDbContext db)
    {
        if (await db.CreditLimits.AnyAsync()) return;

        var credit = CreditLimit.Create(new CreateCreditLimitArg(1, 102, 5000000));
        await db.CreditLimits.AddAsync(credit);
        await db.SaveChangesAsync();
        Console.WriteLine("  ➕ اعتبار اقساطی پیش‌فرض Seed شد: مشتری (Id=102) → سقف 5,000,000");
    }
}
