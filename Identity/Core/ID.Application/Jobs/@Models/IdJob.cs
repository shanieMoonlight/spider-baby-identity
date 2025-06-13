namespace ID.Application.Jobs.Models;

public class IdJobDto(string type, string method, IEnumerable<object> args)
{
    public string Type { get; set; } = type;

    public string Method { get; set; } = method;

    public IEnumerable<object> Args { get; set; } = args;

}//Cls