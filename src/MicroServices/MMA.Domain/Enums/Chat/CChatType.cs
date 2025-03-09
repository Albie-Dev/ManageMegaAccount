using System.ComponentModel;

namespace MMA.Domain
{
    public enum CChatType
    {
        [Description(description: "Không xác định")]
        None = 0,
        [Description(description: "Trò chuyện của tôi")]
        Self = 1,
        [Description(description: "Đối thoại")]
        Dialogue = 2,
        [Description(description: "Trò chuyện nhóm")]
        Group = 3,
    }
}