using System.Net;
using System.Text.Json;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.Exceptions;

namespace EcommerceWepApi.API.Middleware
{
    /// <summary>
    /// معالج الأخطاء العام - يلتقط جميع الاستثناءات ويرجع رسالة مناسبة
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var (statusCode, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                BadRequestException => (HttpStatusCode.BadRequest, exception.Message),
                UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message),
                ForbiddenException => (HttpStatusCode.Forbidden, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "حدث خطأ داخلي في الخادم")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.FailureResponse(message);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}