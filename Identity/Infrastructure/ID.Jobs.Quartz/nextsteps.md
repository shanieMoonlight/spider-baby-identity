# Next steps — ID.Jobs.Quartz

This file summarizes recommended fixes, improvements and follow-ups from the review of the `ID.Jobs.Quartz` library.

## Immediate fixes
- [x] Ensure `HandlerAdapter<THandler>` behavior is final (already updated): throw on unsupported handler signature and cache throwing sentinel.
- [ ] Confirm `IsolatedQuartzExtensions` calls the exact provider methods and signatures (SqlServer vs Postgres).
- [x] Standardize `QuartzConstants.Schema` usage across the project (one constant name).
- [ ] Confirm `HandlerAdapter<>` is covered by unit tests for supported and unsupported signatures.

## DbUp & migrations
- [x] Embed migrations via csproj wildcards (done).
- [x] Standardize token format used in SQL templates (recommend: `${schema}`) or keep migrator substitution for both `${schema}` and `$schema$`.
- [ ] Prefer preprocessing embedded scripts to create `SqlScript` objects with file-like names (keeps journal entries clean).
- [ ] Ensure `JournalToSqlTable` schema/table exists or migrator can create it — document required DB privileges.
- [ ] Set `@DropDb = 0` by default in scripts; destructive statements must be explicit and opt-in.
- [ ] Add support for marking scripts as non-transactional if required by provider.

## Postgres provider
- [x] Populate `Persistence/Initializers/Postgres/Migrations/001-create-quartz-<ver>.sql` with official Quartz PostgreSQL DDL for the version in use.
- [ ] Use `search_path` in Postgres (connection string or script header) so vendor SQL with unqualified names works without rewriting every statement.
- [ ] Verify Postgres provider method name (`UsePostgres`/`UsePostgreSql`/`UseNpgsql`) and set `TablePrefix`/`Schema` appropriately.

## Orchestration & configuration
- [ ] Make migrations opt‑in by default (set `ensureDb = false` in public APIs).
- [ ] Recommend running migrations in CI/CD or a single admin process rather than every instance at startup; document runbook.
- [ ] Add an optional configuration switch to let consumers choose sentinel behavior (throw vs no-op) — default to `throw`.

> Note: For now the project retains `ensureDb = true` by default in `AddMyIdQuartzJobs(...)`. This is intentional for development convenience but must be reviewed before production deployment. Recommended production practice: run DbUp migrations explicitly in CI/CD or a designated admin step and keep runtime automatic schema changes disabled unless explicitly approved.

## Logging & diagnostics
- [x] Capture DbUp output via `ILogger` (not only console) so migration activity appears in application logs.
- [x] Add a startup diagnostic mode (dev) that lists embedded resource names and which scripts match the migrations filter.
- [x] Log which scripts were applied/skipped and journal entries for easier debugging.

## Tests & CI
- [ ] Add integration tests that run migrations against disposable containers (SQL Server + Postgres) and verify schema creation.
- [ ] Add a CI job (GitHub Actions) that runs migrations and a small scheduler smoke test (persist a job, read it back).
- [ ] Add unit tests for the `HandlerAdapter` covering supported signatures and the unsupported case (ensures thrown exception and cached sentinel behavior).

## Documentation
- [ ] Add `README.md` describing:
  - how to opt-in to DB initialization (`ensureDb`),
  - required DB privileges,
  - migration naming convention (zero-padded numeric prefixes),
  - how to add new provider migrations,
  - recommended production migration process (CI/CD or single-run).
- [ ] Add short runbook: how to roll back, backups, and how to validate after migration.

## Operational recommendations
- [ ] Require backups and have rollback/restore plan before running migrations in production.
- [ ] Prefer schema isolation (recommended) or unique table prefix if schema not available.
- [ ] For clustered deployments: ensure only one designated process applies migrations.

## Small housekeeping / code hygiene
- [ ] Remove duplicate/leftover SQL blocks in templates.
- [ ] Consider renaming templates to `.sql.template` if you want to avoid editor/SSDT parsing warnings.
- [ ] Add a dev-only command or small test helper to print `Assembly.GetManifestResourceNames()` to verify embedded resource names.

---

