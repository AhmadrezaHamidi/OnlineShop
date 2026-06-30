using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Entities;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Bnpl;

public sealed class BnplContractConfiguration : IEntityTypeConfiguration<BnplContract>
{
    public void Configure(EntityTypeBuilder<BnplContract> builder)
    {
        builder.ToTable("BnplContracts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.InstallmentCount).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasMany(x => x.Installments)
               .WithOne()
               .HasForeignKey(x => x.ContractId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.Status);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class InstallmentConfiguration : IEntityTypeConfiguration<Installment>
{
    public void Configure(EntityTypeBuilder<Installment> builder)
    {
        builder.ToTable("Installments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ContractId).IsRequired();
        builder.Property(x => x.InstallmentNo).IsRequired();
        builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.PaidAt);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(x => x.ContractId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.DueDate);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class CreditLimitConfiguration : IEntityTypeConfiguration<CreditLimit>
{
    public void Configure(EntityTypeBuilder<CreditLimit> builder)
    {
        builder.ToTable("CreditLimits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TotalLimit).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.UsedLimit).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.UpdatedAt).IsRequired();

        // Computed property — نه ستون DB
        builder.Ignore(x => x.AvailableLimit);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
