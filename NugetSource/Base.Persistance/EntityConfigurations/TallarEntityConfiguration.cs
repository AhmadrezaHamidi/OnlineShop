using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Domain.Entities.TallarEntities;

namespace Base.Persistance.EntityConfigurations
{

    public class TallarEntityConfiguration : IEntityTypeConfiguration<Tallar>
    {
        public void Configure(EntityTypeBuilder<Tallar> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Image).IsRequired().HasMaxLength(255);
            // سایر تنظیمات مربوط به Tallar
        }
    }

}
