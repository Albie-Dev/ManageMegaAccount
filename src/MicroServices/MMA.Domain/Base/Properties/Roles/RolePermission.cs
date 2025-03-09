using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.Domain
{
    public class RolePermission
    {
        [Column("LoaiTaiNguyen")]
        public CResourceType ResourceType { get; set; }
        [Column("DanhSachQuyen")]
        public List<CPermissionType> PermissionTypes { get; set; } = new();
    }
}