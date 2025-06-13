using System.Dynamic;
using System.Text;

namespace ID.Application.Mediatr.Behaviours.Validation;


//========================================================================//

public record ValidationErrorInfo(string Key, object Value);


//========================================================================//

public class ValidationFailureReponse
{
    public ExpandoObject Errors { get; private set; } = new();

    //-----------------------------//

    public ValidationFailureReponse AddError(ValidationErrorInfo errorInfo)
    {
        Errors.TryAdd(errorInfo.Key, errorInfo.Value);
        return this;
    }

    //-----------------------------//

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Errors - ");
        foreach (KeyValuePair<string, object?> kvp in Errors)
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");

        return sb.ToString();

    }

}//Cls

//========================================================================//
