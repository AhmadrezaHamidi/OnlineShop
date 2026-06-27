using Ahmad.OnlineShop.Domain.BackOffice.Aggregates;
using Ahmad.OnlineShop.Domain.BackOffice.Entities;
using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Entities;
using Ahmad.OnlineShop.Domain.Products;
using Ahmad.OnlineShop.Domain.User;
using AhmadBase.Doamin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ahmad.OnlineShop.Persistence.EF;

public sealed class ApplicationDbContext
  : IdentityUserContext<
      User,
      long,
      IdentityUserClaim<long>,
      IdentityUserLogin<long>,
      IdentityUserToken<long>>,
    IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<AdminUser>    AdminUsers    => Set<AdminUser>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<BnplContract> BnplContracts => Set<BnplContract>();
    public DbSet<CreditLimit>  CreditLimits  => Set<CreditLimit>();
    public DbSet<Installment>  Installments  => Set<Installment>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
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

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=.;Database=Ahmad.OnlineShopDb;Trusted_Connection=True;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}