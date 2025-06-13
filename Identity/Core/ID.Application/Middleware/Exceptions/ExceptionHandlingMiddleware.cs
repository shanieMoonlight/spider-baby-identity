using ClArch.ValueObjects.Exceptions;
using ID.Application.Utility;
using ID.Domain.Utility.Exceptions;
using LoggingHelpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ID.Application.Middleware.Exceptions;

internal class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IOptions<ExceptionHandlingMiddlewareOptions> iOpts)
{
    private readonly MyIdExceptionConverter _converter = iOpts.Value.Converter;

    //-------------------------------------//

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (MyIdException exception)
        {
            logger.LogException(exception, MyIdLoggingEvents.UNEXPECTED);
            var exceptionDetails = _converter.GetExceptionDetails(exception);
            context.Response.StatusCode = exceptionDetails.Status;
            await context.Response.WriteAsJsonAsync(exceptionDetails.Details);
        }
        catch (InvalidPropertyException exception)
        {
            logger.LogException(exception, MyIdLoggingEvents.DB.InvalidProperty);
            var exceptionDetails = _converter.GetExceptionDetails(exception);
            context.Response.StatusCode = exceptionDetails.Status;
            await context.Response.WriteAsJsonAsync(exceptionDetails.Details);
        }
    }

    //-------------------------------------//

    internal record ExceptionDetails(int Status, object Details);

    //-------------------------------------//

}//Cls

//==================================================================================//

internal static class MyIdExceptionHandlingExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app, MyIdExceptionConverter converter)
    {
        var options = new ExceptionHandlingMiddlewareOptions(converter);
        app.UseMiddleware<ExceptionHandlingMiddleware>(Options.Create(options));
    }

}//Cls

//==================================================================================//

public class ExceptionHandlingMiddlewareOptions(MyIdExceptionConverter converter)
{
    /// <summary>
    /// Converts exception into ExceptionDetails
    /// </summary>
    public MyIdExceptionConverter Converter { get; set; } = converter;
}//Cls


//==================================================================================//