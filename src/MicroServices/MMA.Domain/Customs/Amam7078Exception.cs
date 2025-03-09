namespace MMA.Domain
{
    public class MMAException : Exception
    {
        public int StatusCode { get; set; }
        public List<ErrorDetailDto> Errors { get; set; } = new();
        public MMAException(int statusCode, List<ErrorDetailDto> errors)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}