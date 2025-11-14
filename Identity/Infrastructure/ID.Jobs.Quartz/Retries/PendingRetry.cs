internal sealed record PendingRetry(
    Func<CancellationToken, Task> Action,   // async work to try    
    string? Description = null,             // human-friendly info for logs/metrics
    DateTimeOffset EnqueuedAt = default     // optional timestamp
)
{
    /// <summary>
    /// Maximum number of attempts for this pending retry.
    /// </summary>
    public int MaxAttempts { get; init; }  = 2;
};