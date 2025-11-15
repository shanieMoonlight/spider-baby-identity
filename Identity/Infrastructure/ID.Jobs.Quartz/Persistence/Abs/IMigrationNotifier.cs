namespace ID.Jobs.Quartz.Persistence.Abs;

internal interface IMigrationNotifier
{
    /// <summary>
    /// Register or replace the single migrations-succeeded handler. Use null to clear.
    /// The handler will be invoked (at most once) when <see cref="NotifySucceededAsync"/> is called.
    /// </summary>
    void SetMigrationsSucceededHandler(Func<CancellationToken, Task>? handler);

    /// <summary>
    /// Notify the registered handler that migrations succeeded. Best-effort: failures in the handler will be caught by the notifier.
    /// </summary>
    Task NotifySucceededAsync(CancellationToken cancellationToken = default);
}
