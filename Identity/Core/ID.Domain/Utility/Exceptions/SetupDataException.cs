namespace ID.Domain.Utility.Exceptions;
public class SetupDataException : MyIdException
{
    public SetupDataException(string msg) : base(msg) { }
    public SetupDataException(string methodName, string missingDataName)
        : base($"Setup data, {missingDataName} was not present when calling {methodName}")
    { }

}//Cls

public class InvalidSetupDataException(string appSection, string invalidDataName, string reason)
    : MyIdException($"MyId.{nameof(appSection)}: Invalid setup data: {invalidDataName}. {reason}")
{ }


