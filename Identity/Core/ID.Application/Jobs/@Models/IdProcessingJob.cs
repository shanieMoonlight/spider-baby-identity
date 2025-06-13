namespace ID.Application.Jobs.Models;

public class IdProcessingJob
{
    public required IdJobDto Job { get; set; }

    public Exception? LoadException { get; set; }

    public required string ServerId { get; set; }

    public DateTime? StartedAt { get; set; }


    private IDictionary<string, string> _stateData = new Dictionary<string, string>();
    public IDictionary<string, string> StateData
    {
        get => _stateData;
        set => _stateData = value ?? new Dictionary<string, string>();
    }

}//Cls

