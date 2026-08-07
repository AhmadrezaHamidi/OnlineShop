using Ahmad.OnlineShop.Domain.Invoice.Aggregates;
using Ahmad.OnlineShop.Domain.Invoice.Entities;
using Ahmad.OnlineShop.Domain.Invoice.Enums;
using Ahmad.OnlineShop.Domain.Order.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Invoice;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Domain.Invoice.Aggregates.Invoice>
{
    public void Configure(EntityTypeBuilder<Domain.Invoice.Aggregates.Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.InvoiceNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.HasIndex(x => x.OrderId).IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(x => x.PaymentMethod)
            .HasConversion<string>().HasMaxLength(30).IsRequired();

        builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PackageDiscount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");

        builder.Property(x => x.DiscountCode).HasMaxLength(50);
        builder.Property(x => x.TransactionCode).HasMaxLength(100);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(i => i.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Ignore(x => x.TotalPrice);
    }
}
