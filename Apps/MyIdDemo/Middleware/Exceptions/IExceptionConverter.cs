namespace MyIdDemo.Middleware.Exceptions;

//#############################//

public record ExceptionDetails(int Status, object Details);

//#############################//


/// <summary>
/// Converts exception into ExceptionDetails
/// </summary>
public interface IExceptionConverter
{
    public ExceptionDetails GetExceptionDetails(Exception exception);

}//Cls