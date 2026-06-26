using Ahmad.OnlineShop.Domain.BackOffice.Entities;
using AhmadBase.Persistence.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.BackOffice;

public sealed partial class BackOfficeConfiguration
{
    public sealed class AuditLogConfiguration : BaseEntityMapping<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.AdminUserId);

            builder.Property(x => x.Action)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.EntityType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.EntityId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.OldValue)
                   .HasMaxLength(500);

            builder.Property(x => x.NewValue)
                   .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            // Indexes
            builder.HasIndex(x => x.AdminUserId);
            builder.HasIndex(x => new { x.EntityType, x.EntityId });
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
