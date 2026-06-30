using Ahmad.OnlineShop.Domain.Products;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Products;

public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);
        builder.Property(x => x.BucketKey).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.UploadedAt).IsRequired();

        builder.HasIndex(x => x.ProductId);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
