using System.Net;
using System.Text.Json;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
     private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
      private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
  RequestDelegate next,
     ILogger<GlobalExceptionHandlerMiddleware> logger,
       IWebHostEnvironment env)
        {
       _next = next;
        _logger = logger;
   _env = env;
  }

        public async Task InvokeAsync(HttpContext context)
        {
  try
     {
    await _next(context);
 }
        catch (Exception ex)
            {
_logger.LogError(ex, "Beklenmeyen bir hata oluþtu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
  }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
     context.Response.ContentType = "application/json";
  context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

   var response = new ApiResponse
            {
   Success = false,
             Message = "Sunucu tarafýnda bir hata oluþtu.",
  Errors = _env.IsDevelopment() 
     ? new List<string> { exception.Message, exception.StackTrace ?? "" }
      : new List<string> { "Lütfen daha sonra tekrar deneyiniz." }
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

            await context.Response.WriteAsync(jsonResponse);
 }
    }

// Extension method for easy middleware registration
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
  {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
