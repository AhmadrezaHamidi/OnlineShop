using Ahmad.OnlineShop.Domain.Discount.Aggregates;
using Ahmad.OnlineShop.Domain.Discount.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Discount;

public sealed class ProductPackageConfiguration : IEntityTypeConfiguration<ProductPackage>
{
    public void Configure(EntityTypeBuilder<ProductPackage> builder)
    {
        builder.ToTable("ProductPackages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.DiscountPercent).HasColumnType("decimal(5,2)");

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class PackageItemConfiguration : IEntityTypeConfiguration<PackageItem>
{
    public void Configure(EntityTypeBuilder<PackageItem> builder)
    {
        builder.ToTable("PackageItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
    }
}
