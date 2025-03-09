using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MMA.Service
{
    public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRoleEntity>
    {
        public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
        {
            builder.ToTable("CET_VaiTroNguoiDung");

            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Property(ur => ur.UserId)
                .HasColumnName("NguoiDungId")
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .HasColumnName("VaiTroId")
                .IsRequired();

            builder.Property(ur => ur.RolePermissionProperty)
                .HasColumnName("ChiTietVaiTro")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(ur => ur.RolePermissions);
        }
    }
}