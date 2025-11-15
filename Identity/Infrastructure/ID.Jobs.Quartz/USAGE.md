# Usage: ID.Jobs.Quartz

Quick examples for wiring and using the Quartz integration, scheduling jobs, and inspecting pending retries.

## 1) Register services (Program.cs / Startup)


Important: register your handler type in DI before scheduling

```csharp
   services.TryAddScoped<ProcessMyIdOutboxMsgJob>();
```
or
```csharp
   services.TryAddScoped<IProcessMyIdOutboxMsgJob, ProcessMyIdOutboxMsgJob>();  (If using an interface as the Handler Type)
```
Then register the Quartz jobs services:
```csharp

The `HandlerAdapter<THandler>` resolves the handler from the application's DI container at execution time. If the handler type is not registered the adapter will log an error and the job will not run. Register your handler (recommended lifetimes: `Transient` or `Scoped`) before scheduling:

```csharp
// Program.cs / Startup
services.AddTransient<MyHandler>();

// then schedule
await jobsService.StartRecurringJob<MyHandler>("my.job.id", h => h.HandleAsync(), "0 0/5 * * * ?");
```

Notes:
- If scheduling fails (typically because the Quartz DB/schema isn't ready), `StartRecurringJob` will store a `PendingRetry` in the in-memory `PendingRetryStore` for later retry.
- You do not need to call `StoreFailedJobAsync` yourself — it's done by the service.

## 2) Customizing retry metadata

When a job schedule fails the `PendingRetry` created by the job service uses the `jobId` as the store key and includes a default `MaxAttempts` value. If you need different behaviour, modify `StoreFailedJobAsync` to construct a `PendingRetry` with your preferred `MaxAttempts` (the hosted worker will use that value).

Example (inside `QuartzMyIdJobService`):

```csharp
var pending = new PendingRetry(
    ct => ScheduleRecurringJobCore(jobId, jobLambda, cron),
    Description: $"StartRecurringJob {jobId}",
    EnqueuedAt: DateTimeOffset.UtcNow
) { MaxAttempts = 5 };

_store.TryAdd(jobId, pending);
```

## 3) How retries are executed

- After DbUp migrations complete `QuartzDbMigrator` calls `IMigrationNotifier.NotifySucceededAsync(...)`.
- `PendingRetriesHostedService` is registered as a migration handler and will snapshot the `PendingRetryStore`.
- Each item is executed under a Polly retry policy (exponential backoff + jitter). On success the item is removed; on exhaustion the item remains in the store for future migration events.

## 4) Inspecting pending retries (diagnostic)

You can inspect the in-memory pending store (useful for diagnostics or a management endpoint):

```csharp
public class JobsAdminController : ControllerBase
{
    private readonly PendingRetryStore _store;
    public JobsAdminController(PendingRetryStore store) => _store = store;

    [HttpGet("/admin/pending-retries")]
    public IActionResult GetPending()
    {
        var snapshot = _store.Snapshot(); // returns KeyValuePair<string, PendingRetry>[]
        return Ok(snapshot.Select(kv => new { JobId = kv.Key, kv.Value.Description, kv.Value.EnqueuedAt, kv.Value.MaxAttempts }));
    }
}
```

## 5) Registering a migration handler directly (advanced)

The library exposes a single-handler `IMigrationNotifier` API. If you want to run other work when migrations succeed you can register a handler:

```csharp
var notifier = app.Services.GetRequiredService<IMigrationNotifier>();
notifier.SetMigrationsSucceededHandler(async ct =>
{
    // run background work or trigger additional processing
    await SomePrewarm(ct);
});
```

Important: the notifier supports a single registered async handler. Use the hosted worker pattern for heavier or multiple consumers.

## 6) Durability

This design uses an in-memory store. If you require durability across process restarts replace `PendingRetryStore` with a persistent implementation (DB table or message queue) and keep the hosted worker logic.

---

If you want I can add a simple management endpoint for pending retries and a sample durable store implementation (SQLite) as a reference — tell me which and I'll add it.