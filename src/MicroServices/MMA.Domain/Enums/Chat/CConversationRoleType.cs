using System.ComponentModel;

namespace MMA.Domain
{
    public enum CConversationRoleType
    {
        [Description(description: "Không xác định")]
        None = 0,
        [Description(description: "Chủ sở hữu")]
        Owner = 1,
        [Description(description: "Quản trị")]
        Admin = 2,
        [Description(description: "Bình thường")]
        Normal = 3
    }
}