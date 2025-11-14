using ID.Jobs.Quartz.Persistence.MigrationNotifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ID.Jobs.Quartz.Retries;

internal sealed class PendingRetriesHostedService(
    PendingRetryStore store,
    IMigrationNotifier notifier,
    ILogger<PendingRetriesHostedService> logger) : BackgroundService
{
    private readonly TimeSpan _maxDelay = TimeSpan.FromSeconds(30);
    private readonly Random _rng = new();

    //-----------------------//

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Register single handler that will be invoked when migrations succeed
        notifier.SetMigrationsSucceededHandler(OnMigrationsSucceededAsync);

        // Clear handler on cancellation
        stoppingToken.Register(() => notifier.SetMigrationsSucceededHandler(null));

        return Task.CompletedTask;
    }

    //-----------------------//

    // Called when migrations succeed (or if called manually for retry)
    private async Task OnMigrationsSucceededAsync(CancellationToken ct)
    {

        // Snapshot store so we don't iterate over a moving collection
        var snapshot = store.Snapshot();

        foreach (var kv in snapshot)
        {
            if (ct.IsCancellationRequested) break;

            var id = kv.Key;
            var item = kv.Value;

            try
            {
                await ProcessWithPolicyAsync(id, item, ct);
            }
            catch (Exception ex)
            {
                // ProcessWithPolicyAsync swallows retry exceptions; log as last resort
                logger.LogWarning(ex, "Unhandled error attempting pending retry for {Id}", id);
            }
        }
    }

    //-----------------------//

    private async Task ProcessWithPolicyAsync(string id, PendingRetry item, CancellationToken ct)
    {
        // Build retry policy with exponential backoff + jitter matching existing ComputeBackoff
        var delays = Enumerable.Range(1, item.MaxAttempts)
            .Select(i =>
            {
                var secs = Math.Pow(2, i);
                var capped = Math.Min(secs, _maxDelay.TotalSeconds);
                var jitterMs = _rng.Next(0, 500);
                return TimeSpan.FromSeconds(capped) + TimeSpan.FromMilliseconds(jitterMs);
            })
            .ToArray();

        AsyncRetryPolicy policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(delays, (ex, time, retryCount, ctx) =>
            {
                logger.LogWarning(ex, "Retry {Retry} failed for {Desc}, will retry after {Delay}", retryCount, item.Description, time);
            });

        try
        {
            // Execute the action under the Polly policy. If policy exhausts, it will throw the final exception.
            await policy.ExecuteAsync(async ctInner => await item.Action(ctInner), ct);

            // Success -> remove from store
            if (store.TryRemove(id, out _))
                logger.LogInformation("Pending retry succeeded and removed: {Desc}", item.Description);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            // Polly gave up (final exception). We intentionally keep item in store so another
            // migration success will re-try it later. Log and move on.
            logger.LogWarning(ex, "Giving up for now on {Desc}; item remains stored for future attempts.", item.Description);
        }
    }

    //-----------------------//

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            notifier.SetMigrationsSucceededHandler(null);
        }
        catch
        {//Swallow
        }
        return base.StopAsync(cancellationToken);
    }

}//Cls