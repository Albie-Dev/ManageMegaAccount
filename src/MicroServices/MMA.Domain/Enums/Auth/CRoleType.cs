using System.ComponentModel;

namespace MMA.Domain
{
    public enum CRoleType
    {
        [Description(description: "Không xác định")]
        None = 0,
        [Description(description: "Quản trị viên")]
        Admin = 1,
        [Description(description: "Khách")]
        Client = 2
    }
}