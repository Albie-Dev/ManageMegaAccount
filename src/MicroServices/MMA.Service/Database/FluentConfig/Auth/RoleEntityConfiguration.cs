using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MMA.Service
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.HasIndex(us => us.RoleName)
                .IsUnique();

            builder.HasIndex(us => us.RoleType)
                .IsUnique();
        }
    }
}