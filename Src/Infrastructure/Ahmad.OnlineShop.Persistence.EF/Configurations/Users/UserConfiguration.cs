using AhmadBase.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ahmad.OnlineShop.Domain.User;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Users;

public sealed partial class UserConfiguration : BaseEntityMapping<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Family)
            .HasMaxLength(100);

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200);

        builder.Property(x => x.NationalCode)
            .HasMaxLength(10);


        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.IsEnabled)
            .IsRequired();

        builder.HasIndex(x => x.NationalCode)
            .IsUnique(false);


        builder.HasMany(x => x.Sessions)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    
    }
}
