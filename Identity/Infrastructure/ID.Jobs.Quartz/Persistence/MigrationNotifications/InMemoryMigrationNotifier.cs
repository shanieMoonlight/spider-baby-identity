using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Logging;

namespace ID.Jobs.Quartz.Persistence.MigrationNotifications;

/// <summary>
/// In-memory notifier used to signal when Quartz DB migrations have completed successfully.
///
/// Design notes:
/// - This implementation stores a single async handler (a <see cref="Func{CancellationToken, Task}"/>).
/// - The handler can be set or replaced by callers via <see cref="SetMigrationsSucceededHandler"/>.
/// - When <see cref="NotifySucceededAsync"/> is called the notifier will attempt to invoke the
///   registered handler. Invocation is guarded so only one invocation runs at a time.
/// - The implementation deliberately does not permanently remove the handler after a failed
///   invocation. That allows retries: if the handler throws, the handler remains registered so
///   subsequent notifications (e.g. another migration event or a manual retry) can run it again.
/// - Registration and the invoking guard are implemented with lightweight atomic operations
///   (Interlocked/Volatile) to avoid locking.
///
/// Usage guidance:
/// - Typical consumer: register a handler that drains pending retries or pre-warms scheduler.
/// - If you want run-once semantics (clear after first successful run), register a handler that
///   clears itself or call SetMigrationsSucceededHandler(null) from the handler on success.
/// - The notifier is best-effort: handler exceptions are logged but do not propagate to the caller.
/// </summary>
internal sealed class InMemoryMigrationNotifier(ILogger<InMemoryMigrationNotifier> _logger) : IMigrationNotifier
{
    // Single handler storage (may be null when no handler is registered).
    // We avoid direct assignments and use Interlocked.Exchange for atomic replace semantics.
    private Func<CancellationToken, Task>? _handler;

    // Simple integer guard used as a lightweight mutex to prevent concurrent handler invocations.
    // 0 == not invoking, 1 == invoking. We use Interlocked.Exchange to set/reset this flag.
    private int _invoking;

    //-----------------------//

    /// <summary>
    /// Register or replace the single migrations-succeeded handler. Passing <c>null</c> clears it.
    ///
    /// This uses an atomic exchange so callers can replace the handler concurrently without
    /// races. The method does not invoke the handler.
    /// </summary>
    public void SetMigrationsSucceededHandler(Func<CancellationToken, Task>? handler) =>
        Interlocked.Exchange(ref _handler, handler);

    //-----------------------//

    /// <summary>
    /// Notify the registered handler that migrations succeeded.
    ///
    /// Behaviour:
    /// - If no handler is registered nothing happens.
    /// - If another invocation is currently running this call returns immediately (the running
    ///   invocation will handle the work).
    /// - Exceptions thrown by the handler are caught and logged; they do not bubble up to the caller.
    /// - The handler is left registered after invocation (even if it throws). This allows retries
    ///   across subsequent notification calls. If you want the handler removed on success, the
    ///   handler itself should call <see cref="SetMigrationsSucceededHandler"/>(null) or the
    ///   caller can replace/clear it explicitly.
    /// </summary>
    public async Task NotifySucceededAsync(CancellationToken cancellationToken = default)
    {
        // Take a snapshot of the handler reference in a thread-safe manner so it cannot be
        // observed halfway through an assignment. Volatile.Read ensures we see a consistent
        // up-to-date reference.
        var handler = Volatile.Read(ref _handler);
        if (handler == null)
            return; // nothing to do

        // Ensure only one invocation runs at a time. If another thread is already invoking,
        // we skip because that invocation will do the work. This prevents concurrent runs of
        // the handler which may not be safe.
        if (Interlocked.Exchange(ref _invoking, 1) == 1)
            return;

        try
        {
            // Invoke the handler. We intentionally await here so the caller of NotifySucceededAsync
            // can observe completion if they choose to (the caller generally catches/logs failures).
            await handler(cancellationToken);
        }
        catch (Exception ex)
        {
            // Best-effort: log and swallow. We do not clear the handler so a future Notify call
            // can attempt the work again (useful if the handler failed due to transient conditions).
            _logger.LogError(ex, "Error occurred while notifying migration success.");
        }
        finally
        {
            // Release the invoking guard so future notifications may run the handler.
            Interlocked.Exchange(ref _invoking, 0);
        }
    }

}//Cls







