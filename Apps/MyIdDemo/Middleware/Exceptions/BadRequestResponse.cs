using System.Dynamic;
using System.Text;

namespace MyIdDemo.Middleware.Exceptions;

public class BadRequestResponse
{
    public ExpandoObject Errors { get; private set; } = new();
    public bool Successful { get; set; }

    //-----------------------------//


    private BadRequestResponse() { }
    public static BadRequestResponse Create() => new();


    //-----------------------------//


    public BadRequestResponse AddError(string key, object error)
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
