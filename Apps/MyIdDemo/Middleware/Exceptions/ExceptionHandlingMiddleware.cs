using LoggingHelpers;
using Microsoft.Extensions.Options;

namespace MyIdDemo.Middleware.Exceptions;

//#############################//


public record ExceptionHandlingMiddlewareOptions(IExceptionConverter Converter);


//#############################//


public class ExceptionHandlingMiddleware(
    RequestDelegate _next,
    ILogger<ExceptionHandlingMiddleware> _logger,
    IOptions<ExceptionHandlingMiddlewareOptions> iOpts)
{

    public async Task InvokeAsync(HttpContext context)
    {
        IExceptionConverter _converter = iOpts.Value.Converter;
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {

            _logger.LogException(exception, 215);
            var exceptionDetails = _converter.GetExceptionDetails(exception);
            context.Response.StatusCode = exceptionDetails.Status;
            await context.Response.WriteAsJsonAsync(exceptionDetails.Details);
        }

    }

}//Cls



//#############################//


public static class ApplicationBuilderExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app, IExceptionConverter converter)
    {
        var options = new ExceptionHandlingMiddlewareOptions(converter);

        app.UseMiddleware<ExceptionHandlingMiddleware>(Options.Create(options));
    }

}//Cls

