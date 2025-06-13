namespace ID.Application.Jobs.@Abstractions;

/// <summary>
/// All Jobs need a JobId. 
/// This is a helper class
/// </summary>
public abstract class AMyIdJobHandler(string jobId)
{
    public string JobId { get; set; } = $"MY_ID_{jobId}";
}