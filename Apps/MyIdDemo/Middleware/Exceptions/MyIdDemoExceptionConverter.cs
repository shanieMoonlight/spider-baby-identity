using ClArch.ValueObjects.Exceptions;
using ControllerHelpers.Responses;
using ID.Domain.Utility.Exceptions;

namespace MyIdDemo.Middleware.Exceptions;


/// <summary>
/// Convert exception into an information Object
/// </summary>
public class MyIdDemoExceptionConverter : IExceptionConverter
{
    /// <summary>
    /// Convert exception into an information Object
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            InvalidPropertyException ipe => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                BadRequestResponse.Create().AddError(ipe.Property, ipe.Message)),

            CantDeleteException cde => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                BadRequestResponse.Create().AddError(cde.Property, cde.Message)),

            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                MessageResponseDto.Generate(exception.Message))
        };

    }


}//Cls