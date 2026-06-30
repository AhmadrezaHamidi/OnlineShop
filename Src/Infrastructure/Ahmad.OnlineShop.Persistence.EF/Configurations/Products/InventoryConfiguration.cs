using Ahmad.OnlineShop.Domain.Products;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Products;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.ReservedQuantity).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // Computed property — نه ستون DB
        builder.Ignore(x => x.AvailableQuantity);
        builder.Ignore(x => x.IsLowStock);
        builder.Ignore(x => x.IsOutOfStock);

        builder.HasIndex(x => x.ProductId).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
