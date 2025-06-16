namespace MyIdDemo.Utils;

public static class HttpExtensions
{
    internal static bool IsSwaggerRequest(this HttpRequest request)
    {
        // Swagger requests are typically made to the /swagger endpoint or similar
        return request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
            || request.Path.StartsWithSegments("/api-docs", StringComparison.OrdinalIgnoreCase);
    }


}
