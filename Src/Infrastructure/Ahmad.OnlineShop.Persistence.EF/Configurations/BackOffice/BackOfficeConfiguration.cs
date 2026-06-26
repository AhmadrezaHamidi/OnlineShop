using Ahmad.OnlineShop.Domain.BackOffice.Aggregates;
using AhmadBase.Persistence.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.BackOffice;

public sealed partial class BackOfficeConfiguration
{
    public sealed class AdminUserConfiguration : BaseEntityMapping<AdminUser>
    {
        public void Configure(EntityTypeBuilder<AdminUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.FullName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(x => x.Role)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            // Indexes
            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.HasIndex(x => x.Status);
        }
    }
}
