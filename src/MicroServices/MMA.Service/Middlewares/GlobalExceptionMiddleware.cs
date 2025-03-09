using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MMA.Domain;

namespace MMA.Service
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (MMAException ex)
            {
                _logger.LogError(ex, "An error occurred.");
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var result = new ResponseResult<object>
                {
                    Success = false,
                    Data = null,
                    Errors = ex.Errors
                };

                await context.Response.WriteAsync(result.ToJson());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var result = new ResponseResult<object>
                {
                    Success = false,
                    Data = null,
                    Errors = new List<ErrorDetailDto>
                    {
                        new ErrorDetailDto()
                        {
                            Error = $"Đã có lỗi xảy ra : {ex.Message}",
                            ErrorScope = CErrorScope.PageSumarry
                        }
                    }
                };

                await context.Response.WriteAsync(result.ToJson());
            }
        }
    }
}