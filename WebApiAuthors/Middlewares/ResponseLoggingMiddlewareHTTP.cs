namespace WebApiAuthors.Middlewares;

public static class ResponseLoggingMiddlewareHTTPExtensions
{
    public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ResponseLoggingMiddlewareHTTP>();
    }
}

public class ResponseLoggingMiddlewareHTTP
{
    private readonly ILogger<ResponseLoggingMiddlewareHTTP> _logger;
    private readonly RequestDelegate _next;

    public ResponseLoggingMiddlewareHTTP(RequestDelegate next, ILogger<ResponseLoggingMiddlewareHTTP> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Invoke o InvokeAsync

    public async Task InvokeAsync(HttpContext context)
    {
        using (var ms = new MemoryStream())
        {
            var responseOriginalBody = context.Response.Body;
            context.Response.Body = ms;
            await _next(context);

            ms.Seek(0, SeekOrigin.Begin);
            var response = new StreamReader(ms).ReadToEndAsync();
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(responseOriginalBody);
            context.Response.Body = responseOriginalBody;

            _logger.LogInformation(await response);
        }
    }
}