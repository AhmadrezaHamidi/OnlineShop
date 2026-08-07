using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Persistance.EntityConfigurations
{
    public class MissionEntityConfiguration : IEntityTypeConfiguration<MissionEntity>
    {
        public void Configure(EntityTypeBuilder<MissionEntity> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Title).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Description);

            builder.HasOne(m => m.Employee)
                .WithMany(e => e.Mission)
                .HasForeignKey(m => m.EmployeeId);
        }
    }
}
