using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Base.Domain.Entities.TallarEntities;

namespace Base.Persistance.EntityConfigurations
{
    public class OptionsTallarEntityConfiguration : IEntityTypeConfiguration<OptionsTallar>
    {
        public void Configure(EntityTypeBuilder<OptionsTallar> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(100);
            builder.Property(e => e.IsDeleted).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasOne(e => e.Tallar)
                .WithMany(t => t.Optiones)
                .HasForeignKey(e => e.TallarId);
        }
    }

}
