namespace ID.Application.Jobs.Models;

public class IdRecurringJob
{
    public required string Id { get; set; } = string.Empty;
    public required IdJobDto Job { get; set; }

    public required string Cron { get; set; }

    public string? Queue { get; set; }


    public Exception? LoadException { get; set; }

    public DateTime? NextExecution { get; set; }

    public string? LastJobId { get; set; }

    public string? LastJobState { get; set; }

    public DateTime? LastExecution { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool Removed { get; set; }

    public string? TimeZoneId { get; set; }

    public string? Error { get; set; }

    public int RetryAttempt { get; set; } = 0;

}//Cls

