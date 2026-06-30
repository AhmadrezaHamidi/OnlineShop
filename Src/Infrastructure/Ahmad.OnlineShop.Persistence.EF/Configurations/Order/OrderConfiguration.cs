using Ahmad.OnlineShop.Domain.Order.Aggregates;
using Ahmad.OnlineShop.Domain.Order.Entities;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Order;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Ahmad.OnlineShop.Domain.Order.Aggregates.Order>
{
    public void Configure(EntityTypeBuilder<Ahmad.OnlineShop.Domain.Order.Aggregates.Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.PaymentMethod)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.PlacedAt).IsRequired();

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Payments)
               .WithOne()
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
    }
}

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

        builder.Ignore(x => x.TotalPrice);

        builder.HasIndex(x => x.OrderId);
    }
}

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Method)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Provider).HasMaxLength(100);
        builder.Property(x => x.Authority).HasMaxLength(200);
        builder.Property(x => x.TransactionCode).HasMaxLength(200);

        builder.Ignore(x => x.IsSuccessful);
        builder.Ignore(x => x.IsCashOnDelivery);

        builder.HasIndex(x => x.OrderId);
    }
}
