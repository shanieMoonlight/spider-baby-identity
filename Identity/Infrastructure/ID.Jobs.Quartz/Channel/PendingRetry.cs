internal sealed record PendingRetry(
    Func<CancellationToken, Task> Action,   // async work to try
    int Attempts = 0,                       // number of times already tried
    string? Description = null,             // human-friendly info for logs/metrics
    DateTimeOffset EnqueuedAt = default     // optional timestamp
);