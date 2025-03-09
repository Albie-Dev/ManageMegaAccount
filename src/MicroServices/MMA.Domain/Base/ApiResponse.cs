namespace MMA.Domain
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public ResponseResult<T> Result { get; set; } = new ResponseResult<T>();
    }

    public class ResponseResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<ErrorDetailDto> Errors { get; set; } = new List<ErrorDetailDto>();
    }

    public class BasePagedResult<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public object? Filter { get; set; }
        public List<T> Items { get; set; } = new();
    }
}