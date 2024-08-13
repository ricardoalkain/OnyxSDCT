
namespace Onyx.ProductService.Endpoints.Authentication;

public class ApiKeyAuthenticationFilter : IEndpointFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConsts.API_KEY_HEADER, out var apiKey))
        {
            return Results.Unauthorized();
        }

        var actualKey = _configuration.GetValue<string>(AuthConsts.API_KEY_CONFIG);
        if (!string.Equals(actualKey, apiKey))
        {
            return Results.Forbid();
        }

        return await next(context);
    }
}
