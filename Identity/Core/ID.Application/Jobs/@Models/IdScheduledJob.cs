namespace ID.Application.Jobs.Models;

public class IdScheduledJob
{
    public required IdJobDto Job { get; set; }

    public Exception? LoadException { get; set; }

    public DateTime EnqueueAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public bool InScheduledState { get; set; }

    private IDictionary<string, string> _stateData = new Dictionary<string, string>();
    public IDictionary<string, string> StateData
    {
        get => _stateData;
        set => _stateData = value ?? new Dictionary<string, string>();
    }
}//Cls

