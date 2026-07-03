using Ahmad.OnlineShop.Domain.Products;
using Ahmad.OnlineShop.Domain.Products.Args;
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
        await SeedCategoriesAsync(appDb);
        await SeedProductsAsync(appDb);
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

    private static async Task SeedCategoriesAsync(ApplicationDbContext db)
    {
        if (await db.Categories.AnyAsync()) return;

        var categories = new[]
        {
            new Category(1, "لوازم جانبی",  null),
            new Category(2, "پوشاک",         null),
            new Category(3, "کفش و کتانی",  null),
            new Category(4, "الکترونیک",     null),
            new Category(5, "ورزش و سلامت", null),
        };

        await db.Categories.AddRangeAsync(categories);
        await db.SaveChangesAsync();
        Console.WriteLine("  ➕ ۵ دسته‌بندی Seed شدند: لوازم جانبی، پوشاک، کفش و کتانی، الکترونیک، ورزش و سلامت");
    }

    private static async Task SeedProductsAsync(ApplicationDbContext db)
    {
        if (await db.Products.AnyAsync()) return;

        const long sellerId = 101; // Ahmad Seller

        // (Id, CategoryId, Name, Description, Price, InventoryId, Stock)
        var specs = new (long Id, long CatId, string Name, string Desc, decimal Price, long InvId, int Stock)[]
        {
            // لوازم جانبی (cat=1)
            (1,  1, "کیف دوشی چرم",              "کیف دوشی ساخته‌شده از چرم طبیعی",            2_800_000m, 201, 28),
            (2,  1, "کوله‌پشتی لپ‌تاپ ۱۵ اینچ", "کوله مقاوم با پوشش ضدآب",                   1_450_000m, 202, 35),
            (3,  1, "جاکلیدی فلزی آلومینیوم",    "جاکلیدی مدرن از آلومینیوم خالص",               150_000m, 203, 120),
            // پوشاک (cat=2)
            (4,  2, "تی‌شرت اسپرت مردانه",       "جنس کتان نخ‌شانه، تناسب اسلیم‌فیت",           320_000m, 204, 60),
            (5,  2, "شلوار جین زنانه استرچ",      "جین استرچ با کمربند کشی، راحت و سبک",          950_000m, 205, 45),
            (6,  2, "هودی دخترانه زمستانی",       "هودی گرم با جیب کانگورو و بند تنظیم‌پذیر",    780_000m, 206, 18),
            (7,  2, "پیراهن رسمی مردانه",         "پیراهن اسلیم‌فیت مناسب جلسات کاری",          1_100_000m, 207, 22),
            // کفش و کتانی (cat=3)
            (8,  3, "کفش ورزشی نایک ایر مکس",    "کفش دوی حرفه‌ای با کفی هوا و زیره لاستیکی", 1_800_000m, 208, 15),
            (9,  3, "کفش کوهنوردی سالومون X",     "کفش مقاوم با زیره ویبرام و ساق بلند",         3_200_000m, 209,  8),
            (10, 3, "کتانی اسپرت آدیداس اولترا",  "کتانی سبک با رویه مش تنفس‌پذیر",             2_100_000m, 210, 24),
            // الکترونیک (cat=4)
            (11, 4, "هدفون بی‌سیم سونی WH-1000",  "نویزکنسلینگ فعال، باتری ۳۰ ساعت",            4_500_000m, 211, 12),
            (12, 4, "پاوربانک ۱۰۰۰۰ میلی‌آمپر",  "شارژ سریع ۱۸W با خروجی USB-C",                680_000m, 212, 50),
            (13, 4, "کابل شارژ USB-C دو متری",    "کابل مقاوم نایلونی ۱۰۰ وات",                   95_000m, 213, 200),
            // ورزش و سلامت (cat=5)
            (14, 5, "دمبل ۵ کیلوگرمی لاستیکی",   "دمبل با پوشش لاستیکی ضدلیز",                  540_000m, 214, 30),
            (15, 5, "یوگا مت حرفه‌ای ۶ میلی‌متر", "مت یوگا با کیسه حمل رایگان",                 890_000m, 215, 18),
        };

        var products = new List<Product>(specs.Length);
        foreach (var s in specs)
        {
            var p = Product.Create(new CreateProductArg(s.Id, sellerId, s.CatId, s.Name, s.Desc, s.Price, s.InvId));
            if (s.Stock > 0)
                p.ReplenishStock(s.Stock);
            products.Add(p);
        }

        await db.Products.AddRangeAsync(products);
        await db.SaveChangesAsync();
        Console.WriteLine($"  ➕ {products.Count} محصول در ۵ دسته‌بندی Seed شدند");
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
