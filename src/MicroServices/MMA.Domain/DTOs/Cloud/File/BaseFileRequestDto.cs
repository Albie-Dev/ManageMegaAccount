namespace MMA.Domain
{
    public class BaseFileRequestDto
    {
        public List<string> FileIds { get; set; } = new List<string>();
        public CCloudType CloudType { get; set; }
    }
}