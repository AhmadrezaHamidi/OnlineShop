using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DiscountAggregate = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Discount;

public sealed class DiscountConfiguration : IEntityTypeConfiguration<DiscountAggregate>
{
    public void Configure(EntityTypeBuilder<DiscountAggregate> builder)
    {
        builder.ToTable("Discounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Value).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinOrderAmount).HasColumnType("decimal(18,2)");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
