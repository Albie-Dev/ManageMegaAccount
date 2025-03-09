using System.ComponentModel;

namespace MMA.Domain
{
    public enum CTypeOfCourse
    {
        [Description(description: "Không xác định")]
        None = 0,
        [Description(description: "Khóa học trực tuyến")]
        VLC = 1,
        [Description(description: "Khóa học trực tiếp")]
        F2F = 2,
        [Description(description: "Khóa học kết hợp giữa học trực tuyến và học trực tiếp")]
        Blended = 3,
        [Description(description: "Khóa học hoàn toàn qua mạng")]
        ELearning = 4,
        [Description(description: "Khóa học kết hợp các khóa học khác nhau thành một gói (gói khóa học).")]
        Bundled = 5,
    }
}