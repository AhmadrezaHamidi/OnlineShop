using Ahmad.OnlineShop.Domain.BackOffice.Entities;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.BackOffice;

public sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.AdminUserId);
        builder.Property(x => x.Type).HasConversion<string>().IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().IsRequired();
        builder.Property(x => x.FilePath).HasMaxLength(500);
        builder.Property(x => x.GeneratedAt);
        builder.Property(x => x.FailReason).HasMaxLength(1000);

        builder.HasIndex(x => x.AdminUserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.GeneratedAt);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
