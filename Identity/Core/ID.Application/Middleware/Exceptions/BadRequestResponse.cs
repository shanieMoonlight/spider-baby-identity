using System.Dynamic;
using System.Text;

namespace ID.Application.Middleware.Exceptions;

public class MyIdBadRequestResponse
{

    public ExpandoObject Errors { get; private set; } = new();
    public bool Successful { get; set; }

    //-----------------------------//
    private MyIdBadRequestResponse() { }

    //-----------------------------//

    public static MyIdBadRequestResponse Create() => new();

    //-----------------------------//

    public MyIdBadRequestResponse AddError(string key, object error)
    {
        Errors.TryAdd(key, error);
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
