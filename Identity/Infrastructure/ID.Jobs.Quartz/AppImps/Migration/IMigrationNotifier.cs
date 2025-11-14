namespace ID.Jobs.Quartz.AppImps.Migration;

internal interface IMigrationNotifier
{
    /// <summary>
    /// Raised when migrations have completed successfully.
    /// Handlers receive a CancellationToken and return a Task so subscribers can perform async work.
    /// </summary>
    event Func<CancellationToken, Task>? MigrationsSucceeded;

    /// <summary>
    /// Notify subscribers that migrations succeeded.
    /// </summary>
    Task NotifySucceededAsync(CancellationToken cancellationToken = default);
}
