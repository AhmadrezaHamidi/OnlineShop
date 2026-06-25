using Ahmad.OnlineShop.Domain.Products;
using AhmadBase.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Products;

public sealed partial class InventoryConfiguration : BaseEntityMapping<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Property(x => x.ProductId)
               .IsRequired();

        builder.Property(x => x.Quantity)
               .IsRequired();

        builder.Property(x => x.ReservedQuantity)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired();

        // Computed Property (Shadow Property)
        builder.Property<int>("AvailableQuantity")
               .HasComputedColumnSql("[Quantity] - [ReservedQuantity]");

        builder.HasIndex(x => x.ProductId)
               .IsUnique(true); // هر محصول فقط یک Inventory دارد
    }
}
