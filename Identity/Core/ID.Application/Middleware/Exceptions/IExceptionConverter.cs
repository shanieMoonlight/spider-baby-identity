namespace ID.Application.Middleware.Exceptions;
public record MyIdExceptionDetails(int Status, object Details);

/// <summary>
/// Converts exception into ExceptionDetails
/// </summary>
public interface IMyIdExceptionConverter
{
    public MyIdExceptionDetails GetExceptionDetails(Exception exception);

}//Cls