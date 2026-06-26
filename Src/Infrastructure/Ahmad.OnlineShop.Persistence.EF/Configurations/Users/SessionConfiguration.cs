using AhmadBase.Persistence.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ahmad.OnlineShop.Domain.User;

namespace Ahmad.OnlineShop.Persistence.EF.Configurations.Users;

public sealed partial class UserConfiguration
{
    public sealed class SessionConfiguration : BaseEntityMapping<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.SessionId)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasIndex(x => x.SessionId)
                .IsUnique();

            builder.HasIndex(x => x.UserId);
        }
    }
 
}
