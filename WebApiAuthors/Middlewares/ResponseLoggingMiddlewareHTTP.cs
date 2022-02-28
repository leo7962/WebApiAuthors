namespace WebApiAuthors.Middlewares;

public static class ResponseLoggingMiddlewareHttpExtensions
{
    public static IApplicationBuilder UseResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ResponseLoggingMiddlewareHttp>();
    }
}

public class ResponseLoggingMiddlewareHttp
{
    private readonly ILogger<ResponseLoggingMiddlewareHttp> _logger;
    private readonly RequestDelegate _next;

    public ResponseLoggingMiddlewareHttp(RequestDelegate next, ILogger<ResponseLoggingMiddlewareHttp> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Invoke o InvokeAsync

    public async Task InvokeAsync(HttpContext context)
    {
        await using var ms = new MemoryStream();
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