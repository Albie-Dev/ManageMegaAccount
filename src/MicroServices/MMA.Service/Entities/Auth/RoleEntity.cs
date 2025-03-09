using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MMA.Domain;

namespace MMA.Service
{
    [Table("CET_VaiTro")]
    public class RoleEntity : BaseEntity
    {
        [Column(name: "TenVaiTro")]
        [StringLength(maximumLength: 20)]
        public string RoleName { get; set; } = string.Empty;
        [Column("LoaiVaiTro")]
        public CRoleType RoleType { get; set; }

        [InverseProperty(property: nameof(UserRoleEntity.Role))]
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    }
}