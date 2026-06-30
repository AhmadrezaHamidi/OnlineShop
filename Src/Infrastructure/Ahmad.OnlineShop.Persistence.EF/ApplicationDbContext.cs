using Ahmad.OnlineShop.Domain.BackOffice.Aggregates;
using Ahmad.OnlineShop.Domain.BackOffice.Entities;
using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Entities;
using Ahmad.OnlineShop.Domain.Products;
using Ahmad.OnlineShop.Domain.User;
using OrderAggregate = Ahmad.OnlineShop.Domain.Order.Aggregates.Order;
using Ahmad.OnlineShop.Domain.Order.Entities;
using AhmadBase.Doamin;
using Identity.Domain.Aggregates;
using Identity.Domain.Entities;
using IdentityAppUser = Identity.Domain.Aggregates.User;
using OldUser = Ahmad.OnlineShop.Domain.User.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ahmad.OnlineShop.Persistence.EF;

/// <summary>
/// DbContext اصلی — شامل Domain entities (محصول، سفارش، BNPL، BackOffice)
/// Identity entities در IdentityAppDbContext جداگانه هستند
/// </summary>
public sealed class ApplicationDbContext
  : IdentityUserContext<
      OldUser,
      long,
      IdentityUserClaim<long>,
      IdentityUserLogin<long>,
      IdentityUserToken<long>>,
    IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── Domain DbSets ─────────────────────────────────────────────────────────
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<BnplContract> BnplContracts => Set<BnplContract>();
    public DbSet<CreditLimit> CreditLimits => Set<CreditLimit>();
    public DbSet<Installment> Installments => Set<Installment>();
    public DbSet<OrderAggregate> Orders => Set<OrderAggregate>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<OldUser>().Ignore(u => u.Roles);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public async Task<int> CommitAsync(CancellationToken token = default)
    {
        var aggregates = ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Any())
            .ToList();

        var result = await SaveChangesAsync(token);

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
                await EventDispatcher.PublishAsync(domainEvent, token);

            aggregate.ClearDomainEvents();
        }

        return result;
    }
}

/// <summary>
/// DbContext جداگانه برای Identity system جدید (OTP-based)
/// User، OtpRequest، RefreshToken، Role از Identity.Domain
/// </summary>
public sealed class IdentityAppDbContext : DbContext, IUnitOfWork
{
    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
        : base(options) { }

    public DbSet<IdentityAppUser> IdentityUsers => Set<IdentityAppUser>();
    public DbSet<OtpRequest> OtpRequests => Set<OtpRequest>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Identity.Domain.Entities.Role> Roles => Set<Identity.Domain.Entities.Role>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityAppUser>(e =>
        {
            e.ToTable("IdentityUsers");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).ValueGeneratedNever();
            e.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
            e.Property(u => u.FullName).HasMaxLength(200);
            e.HasIndex(u => u.PhoneNumber).IsUnique();

            // RoleIds به صورت رشته comma-separated ذخیره می‌شود (backing field: _roleIds)
            var roleIdsConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<IReadOnlyCollection<long>, string>(
                v => string.Join(',', v),
                v => string.IsNullOrEmpty(v)
                    ? new List<long>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList());

            var roleIdsComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyCollection<long>>(
                (a, b) => (a ?? new List<long>()).SequenceEqual(b ?? new List<long>()),
                v => v.Aggregate(0, (hash, val) => HashCode.Combine(hash, val)),
                v => v.ToList());

            e.Property(u => u.RoleIds)
                .HasField("_roleIds")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(roleIdsConverter)
                .HasColumnName("RoleIds")
                .HasMaxLength(500)
                .Metadata.SetValueComparer(roleIdsComparer);
        });

        builder.Entity<OtpRequest>(e =>
        {
            e.ToTable("OtpRequests");
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).ValueGeneratedNever();
            e.Property(o => o.PhoneNumber).HasMaxLength(20).IsRequired();
            e.Property(o => o.Code).HasMaxLength(10).IsRequired();
            e.Ignore(o => o.IsExpired);
            e.Ignore(o => o.IsValid);
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedNever();
            e.Property(r => r.Token).HasMaxLength(500).IsRequired();
            e.HasIndex(r => r.Token).IsUnique();
        });

        builder.Entity<Identity.Domain.Entities.Role>(e =>
        {
            e.ToTable("IdentityRoles");
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedNever();
            e.Property(r => r.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(r => r.Name).IsUnique();
        });
    }

    public async Task<int> CommitAsync(CancellationToken token = default)
        => await SaveChangesAsync(token);
}

// ── Design-time factories ─────────────────────────────────────────────────────

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=Ahmad.OnlineShopDb;Integrated Security=True;TrustServerCertificate=True;");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

public class IdentityAppDbContextFactory : IDesignTimeDbContextFactory<IdentityAppDbContext>
{
    public IdentityAppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityAppDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=Ahmad.OnlineShopDb;Integrated Security=True;TrustServerCertificate=True;");
        return new IdentityAppDbContext(optionsBuilder.Options);
    }
}
