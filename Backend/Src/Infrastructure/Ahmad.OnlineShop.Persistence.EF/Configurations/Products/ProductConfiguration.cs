namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Products;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Ahmad.OnlineShop.Domain.Products.Product>
{
    public void Configure(EntityTypeBuilder<Ahmad.OnlineShop.Domain.Products.Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasConversion<string>();
        builder.Property(x => x.CreationTime).IsRequired();
        builder.Property(x => x.ModificationTime);

        // Inventory جدول جداگانه است (با InventoryConfiguration) — رابطه ۱به۱ بر اساس Inventory.ProductId
        builder.HasOne(p => p.Inventory)
               .WithOne()
               .HasForeignKey<Ahmad.OnlineShop.Domain.Products.Inventory>(i => i.ProductId);

        builder.HasMany(p => p.Images)
               .WithOne()
               .HasForeignKey(i => i.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.CategoryId, x.Status });

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
