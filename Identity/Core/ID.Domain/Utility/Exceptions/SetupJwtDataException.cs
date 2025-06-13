namespace ID.Domain.Utility.Exceptions;
public class SetupJwtDataException : MyIdException
{
    public SetupJwtDataException(string msg) : base(msg) { }
    
    public SetupJwtDataException(string methodName, string missingDataName)
        : base($"JWT Setup data, {missingDataName} was not present when calling {methodName}")
    { }

    public SetupJwtDataException(Exception exception)
    : base($"JWT Setup Error: {exception.Message}", exception)
    { }

}//Cls
