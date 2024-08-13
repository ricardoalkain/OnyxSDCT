namespace Onyx.ProductService.Endpoints.Authentication;

public class ApiKeyAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(AuthConsts.API_KEY_HEADER, out var apiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing API key");
            return;
        }

        var actualKey = _configuration.GetValue<string>(AuthConsts.API_KEY_CONFIG);
        if (!string.Equals(actualKey, apiKey))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        await _next(context);
    }
}
