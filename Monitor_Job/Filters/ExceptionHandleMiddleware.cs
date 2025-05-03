namespace WebApi.Filters
{
    public class ExceptionHandleMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// 任務調用
        /// </summary>
        /// <param name="context">HTTP 的上下文</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return context.Response.WriteAsync(
                $"{context.Response.StatusCode} Internal Server Error from the ExceptionHandle middleware."
            );
        }
    }

    public static class ExceptionHandleMiddlewareExtensions
    {
        /// <summary>在中介程序中全域處理例外</summary>
        /// <param name="builder">中介程序建構器</param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandleMiddleware>();
        }
    }

}
