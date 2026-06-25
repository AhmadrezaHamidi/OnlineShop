using AhmadBase.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ahmad.OnlineShop.Domain.User;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Users;

public sealed partial class UserConfiguration
{
    public sealed class UserRoleConfiguration : BaseEntityMapping<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(x => new { x.UserId, x.RoleId });

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.RoleId)
                .IsRequired();
        }
    }
 
}
