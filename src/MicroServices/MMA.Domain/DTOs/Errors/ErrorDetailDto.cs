namespace MMA.Domain
{
    public class ErrorDetailDto
    {
        public CErrorScope ErrorScope { get; set; }
        public string Field { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}