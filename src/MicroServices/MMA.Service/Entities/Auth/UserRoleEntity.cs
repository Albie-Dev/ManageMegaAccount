using System.ComponentModel.DataAnnotations.Schema;
using MMA.Domain;

namespace MMA.Service
{
    [Table(name: "CET_VaiTroNguoiDung")]
    public class UserRoleEntity : BaseInfo
    {
        [Column(name: "NguoiDungId")]
        public Guid UserId { get; set; }
        [Column(name: "VaiTroId")]
        public Guid RoleId { get; set; }

        [Column(name: "ChiTietVaiTro")]
        public string RolePermissionProperty { get; set; } = string.Empty;
        [NotMapped]
        public List<RolePermission> RolePermissions
        {
            get => RolePermissionProperty.FromJson<List<RolePermission>>();
            set => RolePermissionProperty = RolePermissions.ToJson();
        }

        [ForeignKey(name: nameof(UserRoleEntity.UserId))]
        [InverseProperty(property: nameof(UserEntity.UserRoles))]
        public virtual UserEntity User { get; set; } = null!;

        [ForeignKey(name: nameof(UserRoleEntity.RoleId))]
        [InverseProperty(property: nameof(RoleEntity.UserRoles))]
        public virtual RoleEntity Role { get; set; } = null!;
    }
}