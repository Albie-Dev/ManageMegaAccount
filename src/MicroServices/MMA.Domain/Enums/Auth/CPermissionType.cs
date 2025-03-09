using System.ComponentModel;

namespace MMA.Domain
{
    // 1 người dùng sẽ có nhiều role, 1 role như vậy sẽ có tất cả các resource và tương ứng mỗi resource sẽ có các permission tương ứng.
    public enum CPermissionType
    {
        [Description(description: "Không xác định")]
        None = 0,

        [Description(description: "Đọc")]
        Read = 1,

        [Description(description: "Cập nhật")]
        Update = 2,

        [Description(description: "Xóa")]
        Delete = 3,

        [Description(description: "Tất cả quyền")]
        Manage = Read | Update | Delete
    }

}