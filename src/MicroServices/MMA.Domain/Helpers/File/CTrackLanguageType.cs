using System.ComponentModel;

namespace MMA.Domain
{
    public enum CTrackLanguageType
    {
        None = 0,
        [Description("en")]
        English = 1,
        [Description("jp")]
        Japanese = 2,
        [Description("zh")]
        Chinese = 3,
        [Description("vn")]
        VietNamese = 4,
        [Description("th")]
        Thai = 5
    }
}
