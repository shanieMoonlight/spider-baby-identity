# ID.Jobs.Quartz

This library provides a small, opinionated integration between the application and the Quartz scheduler used by the MyId solution. It includes:

- Job registration and scheduling helpers (`QuartzMyIdJobService`).
- Quartz persistence setup and schema constants.
- Database migration support using DbUp via `QuartzDbMigrator`.
- A lightweight migration notification system so other components can react when Quartz DB migrations complete.
- A simple in-memory pending-retry store and hosted worker to persist and retry failed schedule requests when the DB becomes available.

This README explains the high-level design, the main components, DI registration, and operational notes.

## Goals

- Make scheduling recurring and one-off jobs easy and type-safe (typed handler adapters).
- Ensure the Quartz database schema is migrated at startup (DbUp) and provide a hook for other subsystems when migrations finish.
- When scheduling fails because the DB is not ready, persist pending requests in-memory and retry them after migrations succeed — with backoff and a configurable attempt limit.

## Main components

- `QuartzMyIdJobService` (in `AppImps/JobService`)
  - Public API used by application code to schedule, enqueue and stop jobs.
  - When scheduling fails (typically because Quartz DB isn't ready), the job service stores a `PendingRetry` in the `PendingRetryStore` for later retry.

- `QuartzDbMigrator` (in `AppImps`)
  - Runs DbUp migrations (via `IDbUpMigrator`) against the Quartz schema.
  - After successful migrations it calls into the migration notifier: `IMigrationNotifier.NotifySucceededAsync(...)`.

- `IMigrationNotifier` + `InMemoryMigrationNotifier` (in `Persistence/MigrationNotifications` / `AppImps/Migration`)
  - A lightweight in-process notifier that allows one registered async handler to be invoked when migrations succeed.
  - The project uses a single-handler model (`SetMigrationsSucceededHandler`) with safe atomic registration.
  - The notifier is best-effort: handler exceptions are logged and do not fail the migration step.

- `PendingRetry` and `PendingRetryStore` (in `AppImps/Retries` / `Channel`)
  - `PendingRetry` represents a retryable work item: an async `Action`, a `Description`, `EnqueuedAt`, and `MaxAttempts`.
  - `PendingRetryStore` is an in-memory concurrent dictionary keyed by `jobId` used as the source-of-truth for pending retry items.
  - In this design the `jobId` is used as the unique key so duplicate schedule attempts for the same job are deduplicated.

- `PendingRetriesHostedService` (hosted worker)
  - Subscribes to the migration notifier handler and runs pending items when migrations succeed.
  - Uses `Polly` to execute each retry action with an exponential backoff + jitter retry policy using the `MaxAttempts` from the `PendingRetry`.
  - On success the item is removed from the store; if the policy exhausts it is retained in the store for future migration events.

## DI registration

Call the extension in your startup to wire everything up:

- `services.AddMyIdQuartzJobs(databaseType, connectionString)`
  - Registers `QuartzConfig`, Quartz persistence and app implementations.
  - Registers `IMigrationNotifier` / `InMemoryMigrationNotifier`.
  - Registers the retry store and the `PendingRetriesHostedService`.

- `app.UseMyIdQuartzJobs()`
  - Registers the CrystalQuartz dashboard and any middleware required for the dashboard endpoint.

Files to note:
- `Setup.cs` — public `AddMyIdQuartzJobs` / `UseMyIdQuartzJobs` extension methods.

## Flow: scheduling failure -> retry

1. Application calls `QuartzMyIdJobService.StartRecurringJob(...)`.
2. If scheduling fails due to DB/migration problems the method logs the error and calls `StoreFailedJobAsync(...)`.
3. `StoreFailedJobAsync` creates a `PendingRetry` (keyed by `jobId`) and adds it to `PendingRetryStore`.
4. When `QuartzDbMigrator` completes migrations it calls `_migrationNotifier.NotifySucceededAsync(...)`.
5. `PendingRetriesHostedService` (registered handler) snapshots the store and attempts each pending item using a Polly retry policy.
   - If the policy succeeds the item is removed.
   - If the policy exhausts the item is left in the store for the next migration event.

This approach avoids immediate re-enqueue loops and centralizes retry/backoff policy in one component.

## Why a store instead of a channel?

- Channels are excellent for producer/consumer async pipelines with backpressure. For this use case we decided to use a simple in-memory store because:
  - Pending retries are extremely rare (usually only when the app first boots before DB migrations).
  - We want retries to survive a policy "give up" and be retried on future migration events without being lost.
  - The store approach gives easy deduplication (keyed by `jobId`) and simpler semantics.

If you need durable storage across process restarts, replace `PendingRetryStore` with a persistent store (DB table or message queue) and the hosted worker can process items similarly.

## Implementation notes / thread-safety

- `InMemoryMigrationNotifier` exposes `SetMigrationsSucceededHandler` and stores a single `Func<CancellationToken, Task>` handler. Registration is atomic.
- `NotifySucceededAsync` uses a small atomic `_invoking` guard so only one thread invokes the handler at a time.
  - The handler is not cleared on failure — this allows retry on subsequent notifications.
- `PendingRetryStore` uses `ConcurrentDictionary` for thread-safe add/remove/snapshot.
- `PendingRetriesHostedService` uses `Polly` to implement retry/backoff behavior. The delays include exponential backoff with jitter.

## Extensibility

- Make the retry policy pluggable: replace the internal Polly policy with a policy factory or configuration-driven policy.
- Make `PendingRetryStore` durable by implementing a persisted store (SQL table or message bus). Keep the hosted worker logic identical — it can process persisted items the same way.
- If you later need multiple migration listeners, change `IMigrationNotifier` to a multicast event again and ensure the notifier dispatches handlers in the background (do not block migrations startup for long-running subscribers).

## Testing guidance

- `PendingRetriesHostedService` can be unit-tested by injecting a fake `PendingRetryStore` and `IMigrationNotifier` that invokes the registered handler.
- `QuartzMyIdJobService` behaviour can be tested by mocking `ISchedulerFactory`/`IScheduler` and simulating failures to assert that `PendingRetryStore` receives the expected entries.
- `QuartzDbMigrator` is testable by mocking `IDbUpMigrator` to produce success/failure paths and verifying notifier invocation.

## Operational notes

- This system is in-memory; a process crash means pending retries are lost. For production systems where you cannot lose pending schedule operations you should persist them to durable storage.
- Logging: hosted worker logs each retry attempt and when items are given up for later reattempt.

## FAQ

- Q: Why not call the retry actions directly from the notifier?
  - A: Doing so would either block the migration completion path (if awaited) or require fire-and-forget semantics. Centralizing retries in a hosted worker gives better control, backoff, and observability.

- Q: Will handlers race or be invoked multiple times?
  - A: The notifier uses a simple atomic guard to avoid concurrent invocations. The store-based consumer runs retry logic in a controlled way using Polly.

## Files of interest

- `AppImps/JobService/QuartzMyIdJobService.cs` - The main job service API implementation
- `AppImps/QuartzDbMigrator.cs` - Db Migrator for Quartz schema
- `Persistence/MigrationNotifications/InMemoryMigrationNotifier.cs` - migration notifier implementation. Tell other components when migrations complete.
- `Retries/PendingRetry.cs` - represents a retryable work item
- `Retries/PendingRetriesHostedService.cs` - hosted worker that processes pending retries
- `Retries/PendingRetryStore.cs` - in-memory store for pending retries (Single source of truth)
- `Setup.cs` - DI wiring and `AddMyIdQuartzJobs` extension


