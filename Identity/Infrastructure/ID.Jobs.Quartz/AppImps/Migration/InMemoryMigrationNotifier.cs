namespace ID.Jobs.Quartz.AppImps.Migration;

internal sealed class InMemoryMigrationNotifier() : IMigrationNotifier
{
    public event Func<CancellationToken, Task>? MigrationsSucceeded;

    public Task NotifySucceededAsync(CancellationToken cancellationToken = default)
    {
        var handlers = MigrationsSucceeded;
        if (handlers == null)
            return Task.CompletedTask;


        return InvokeSequentiallyAsync(handlers.GetInvocationList().Cast<Func<CancellationToken, Task>>(), cancellationToken);
    }

    private static async Task InvokeSequentiallyAsync(IEnumerable<Func<CancellationToken, Task>> handlers, CancellationToken ct)
    {
        foreach (var h in handlers)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                await h(ct).ConfigureAwait(false);
            }
            catch
            {
                // Swallow subscriber exceptions here; subscribers should log. Do not let one subscriber break the notification chain.
            }
        }
    }
}




