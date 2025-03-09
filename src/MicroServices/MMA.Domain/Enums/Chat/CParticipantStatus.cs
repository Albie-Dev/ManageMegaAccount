using System.ComponentModel;

namespace MMA.Domain
{
    public enum CParticipantStatus
    {
        [Description(description: "Không xác định")]
        None = 0,
        [Description(description: "Đang trong nhóm")]
        Active = 1,
        [Description(description: "Đang chờ phê duyệt")]
        Pending = 2,
        [Description(description: "Đã rời khỏi cuộc hội thoại")]
        Leave = 3,
        [Description(description: "Đã bị chặn")]
        Banned = 4
    }
}