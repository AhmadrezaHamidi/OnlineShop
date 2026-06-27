using Ahmad.OnlineShop.Domain.Bnpl.Aggregates;
using Ahmad.OnlineShop.Domain.Bnpl.Entities;
using AhmadBase.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Bnpl;

public sealed partial class BnplConfiguration
{
    public sealed class BnplContractConfiguration : BaseEntityMapping<BnplContract>
    {
        public void Configure(EntityTypeBuilder<BnplContract> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.TotalAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.InstallmentCount).IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasMany(x => x.Installments)
                   .WithOne()
                   .HasForeignKey(x => x.ContractId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.Status);
        }
    }

    public sealed class InstallmentConfiguration : BaseEntityMapping<Installment>
    {
        public void Configure(EntityTypeBuilder<Installment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.ContractId).IsRequired();
            builder.Property(x => x.InstallmentNo).IsRequired();

            builder.Property(x => x.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DueDate).IsRequired();
            builder.Property(x => x.PaidAt);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(x => x.ContractId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.DueDate);
        }
    }

    public sealed class CreditLimitConfiguration : BaseEntityMapping<CreditLimit>
    {
        public void Configure(EntityTypeBuilder<CreditLimit> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.TotalLimit)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.UsedLimit)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.UpdatedAt).IsRequired();

            builder.HasIndex(x => x.UserId).IsUnique();
        }
    }
}
