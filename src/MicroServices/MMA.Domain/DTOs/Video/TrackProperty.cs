namespace MMA.Domain
{
    public class TrackProperty
    {
        public string TrackName { get; set; } = string.Empty;
        public string TrackSource { get; set; } = string.Empty;
        public CTrackLanguageType TrackLanguageType { get; set; }
        public string TrackKind { get; set; } = "subtitles";
        public bool IsDefault { get; set; } = false;
    }
}