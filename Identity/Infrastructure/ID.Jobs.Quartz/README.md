# ID.Jobs.Quartz

This library provides a small, opinionated integration between the application and the Quartz scheduler used by the MyId solution. It includes:

- Job registration and scheduling helpers (`QuartzMyIdJobService`).
- Quartz persistence setup and schema constants.
- Database migration support using provider-specific EF-style migrators (`SqlEfCoreMigrator`, `PgEfCoreMigrator`).
- A lightweight migration notification system so other components can react when Quartz DB migrations complete.
- A simple in-memory pending-retry store and hosted worker to persist and retry failed schedule requests when the DB becomes available.

This README explains the high-level design, the main components, DI registration, and operational notes.

## Goals

- Make scheduling recurring and one-off jobs easy and type-safe (typed handler adapters).
- Ensure the Quartz database schema is migrated at startup and provide a hook for other subsystems when migrations finish.
- When scheduling fails because the DB is not ready, persist pending requests in-memory and retry them after migrations succeed — with backoff and a configurable attempt limit.

## Main components

- `QuartzMyIdJobService` (in `AppImps/JobService`)
  - Public API used by application code to schedule, enqueue and stop jobs.
  - When scheduling fails (typically because Quartz DB isn't ready), the job service stores a `PendingRetry` in the `PendingRetryStore` for later retry.

- EF-style migrators (`SqlEfCoreMigrator`, `PgEfCoreMigrator`) in `Persistence/DbUp/*`
  - These are the components responsible for applying the embedded SQL migration scripts for each provider.
  - The migrators use a small runtime abstraction `IDbCommandExecutor` to execute SQL commands so the migrator logic is easily unit-testable (the executor owns the concrete provider connection such as `SqlConnection` or `NpgsqlConnection`).
  - Scripts are loaded from embedded resources via `EmbeddedScriptLoader` which performs simple token replacement.
  - For SQL Server scripts the migrator splits on `GO` batch separators and executes each batch separately before recording a journal entry.
  - After successful application of a migration script the migrator writes a journal row so scripts are not re-applied.

- `IDbCommandExecutor` + implementations
  - `SqlDbCommandExecutor` and `PgDbCommandExecutor` provide a small, testable surface to open connections and execute scalar / non-query commands.
  - The executor manages connection lifetime and parameter creation. Tests can inject a factory that returns a fake `DbConnection` or mock the executor itself.

- `IMigrationNotifier` + `InMemoryMigrationNotifier` (in `Persistence/MigrationNotifications`)
  - A lightweight in-process notifier that allows one registered async handler to be invoked when migrations succeed.
  - The project uses a single-handler model (`SetMigrationsSucceededHandler`) with safe atomic registration.
  - The notifier is best-effort: handler exceptions are logged and do not fail the migration step.

- `PendingRetry` and `PendingRetryStore`
  - `PendingRetry` represents a retryable work item: an async `Action`, a `Description`, `EnqueuedAt`, and `MaxAttempts`.
  - `PendingRetryStore` is an in-memory concurrent dictionary keyed by `jobId` used as the source-of-truth for pending retry items.

- `PendingRetriesHostedService` (hosted worker)
  - Subscribes to the migration notifier handler and runs pending items when migrations succeed.
  - Uses `Polly` to execute each retry action with an exponential backoff + jitter retry policy using the `MaxAttempts` from the `PendingRetry`.
  - On success the item is removed from the store; if the policy exhausts it is retained in the store for future migration events.

## DI registration

Call the extension in your startup to wire everything up:

- `services.AddMyIdQuartzJobs(databaseType, connectionString)` (public setup in `Setup.cs`)
  - Registers `QuartzConfig`, the provider-specific executor and migrator implementations, the embedded script loader, and app implementations.

Internally the package registers the following relevant services (example):

- For SQL Server
  - `IDbCommandExecutor` -> `SqlDbCommandExecutor`
  - `IEfCoreMigrator` -> `SqlEfCoreMigrator`

- For Postgres
  - `IDbCommandExecutor` -> `PgDbCommandExecutor`
  - `IEfCoreMigrator` -> `PgEfCoreMigrator`

- `IMigrationNotifier` -> `InMemoryMigrationNotifier`

Note: the migrators and executors are registered as scoped services; the executor manages a short-lived connection opened when migrations run.

## Migration behaviour details

- Scripts are embedded in the library under `Persistence/DbUp/<provider>/Migrations`.
- The `EmbeddedScriptLoader` performs token substitution (for schema name, etc.) before returning scripts to the migrator.
- SQL Server migrator will split on `GO` batch separators and execute each batch individually; Postgres migrator executes the script as a single command (Postgres scripts may contain DO blocks).
- After a script is applied the migrator inserts a record into the migrations journal table under the configured schema so subsequent runs skip already-applied scripts.

## Testing guidance

- Migrators are unit-testable by mocking `IDbCommandExecutor` and `IEmbeddedScriptLoader` (this project includes a comprehensive suite of unit tests exercising success, skip and failure paths).
- `IDbCommandExecutor` implementations are tested by providing a `Func<DbConnection>` factory that returns a simple fake `DbConnection` used in tests to capture commands and parameters.
- The `EnsureOpenAsync` method on executors is implemented using a small `SemaphoreSlim` to make connection opening safe under concurrency — add concurrency tests where needed.

## Operational notes

- This system is in-memory; a process crash means pending retries are lost. For production systems where you cannot lose pending schedule operations you should persist them to durable storage.
	If you AddOrUpdate (```IMyIdJobService.StartRecurringJob```) jobs on startup this is less of a concern.
- Logging: hosted worker logs each retry attempt and when items are given up for later reattempt.

## Extensibility

- Make the retry policy pluggable: replace the internal Polly policy with a policy factory or configuration-driven policy.
- Make `PendingRetryStore` durable by implementing a persisted store (SQL table or message bus). Keep the hosted worker logic identical — it can process persisted items the same way.
- If you need multiple migration listeners, change `IMigrationNotifier` to a multicast event and ensure the notifier dispatches handlers in the background (do not block migrations startup for long-running subscribers).

## Files of interest

- `AppImps/JobService/QuartzMyIdJobService.cs` - The main job service API implementation
- `AppImps/QuartzDbMigrator.cs` - Migrator orchestration (loads migrator impls and runs them)
- `Persistence/DbUp/*` - provider-specific migrators, embedded scripts and executors
- `Persistence/MigrationNotifications/InMemoryMigrationNotifier.cs` - migration notifier implementation
- `Retries/PendingRetry.cs` - represents a retryable work item
- `Retries/PendingRetriesHostedService.cs` - hosted worker that processes pending retries
- `Retries/PendingRetryStore.cs` - in-memory store for pending retries (Single source of truth)
- `Setup.cs` - DI wiring and `AddMyIdQuartzJobs` extension


