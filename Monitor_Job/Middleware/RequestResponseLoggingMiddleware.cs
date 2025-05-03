using System.Text;

namespace WebApi.Middleware
{
    public class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            // 記錄請求內容
            context.Request.EnableBuffering();
            var requestBody = await ReadRequestBodyAsync(context.Request);
            _logger.LogInformation($"{context.Request.Path} Request: {requestBody}");

            // 進行處理並捕捉回應
            var originalBodyStream = context.Response.Body;
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                // 呼叫下一個中介軟體
                await _next(context);

                // 記錄回應內容
                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
                _logger.LogInformation($"{context.Request.Path} Response: {responseBody}");

                // 重設回應流
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);  // 重設請求流位置
            return body;
        }
    }
}
