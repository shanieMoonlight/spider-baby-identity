using ClArch.ValueObjects.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ID.Application.Middleware.Exceptions;


/// <summary>
/// Convert exception into an information Object
/// </summary>
public class MyIdExceptionConverter : IMyIdExceptionConverter
{
    /// <summary>
    /// Convert exception into an information Object
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public MyIdExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            InvalidPropertyException ipe => new MyIdExceptionDetails(
                StatusCodes.Status400BadRequest,
                MyIdBadRequestResponse.Create().AddError(ipe.Property, ipe.Message)),

            _ => new MyIdExceptionDetails(
                    StatusCodes.Status500InternalServerError,
                    new ErrorMessageResponse(exception.Message)
                )
        };

    }


}//Cls


//========================================//


public record ErrorMessageResponse(string Message);
