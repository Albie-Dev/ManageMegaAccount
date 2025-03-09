using System.ComponentModel;

namespace MMA.Domain
{
    public enum CMigrationStatus
    {
        [Description(description: "Không xác định")]
        None = 0,
        [Description(description: "Đã áp dụng")]
        Applied = 1,
        [Description(description: "Đang chờ")]
        Pending = 2
    }
}