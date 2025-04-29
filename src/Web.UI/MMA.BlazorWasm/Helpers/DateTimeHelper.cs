namespace MMA.BlazorWasm
{
    public static class DateTimeHelper
    {
        public static string GetTimeAgo(DateTimeOffset createdDate)
        {
            var now = DateTimeOffset.UtcNow;
            var diff = now - createdDate;

            if (diff.TotalSeconds < 60)
            {
                return $"{Math.Floor(diff.TotalSeconds)} giây trước";
            }
            if (diff.TotalMinutes < 60)
            {
                return $"{Math.Floor(diff.TotalMinutes)} phút trước";
            }
            if (diff.TotalHours < 24)
            {
                return $"{Math.Floor(diff.TotalHours)} giờ trước";
            }
            if (diff.TotalDays < 30)
            {
                return $"{Math.Floor(diff.TotalDays)} ngày trước";
            }
            if (diff.TotalDays < 365)
            {
                return $"{Math.Floor(diff.TotalDays / 30)} tháng trước";
            }

            return $"{Math.Floor(diff.TotalDays / 365)} năm trước";
        }
    }
}