
Overview
•	Problem: you need EF Core migrations for two providers (SQL Server and Postgres) from the same codebase without leaking provider-specific SQL/types into the other provider.
•	Solution: keep a single domain model and core IdDbContext, but create provider-specific small projects that:
•	provide provider-specific DbContext derived types used only for migrations,
•	include a design-time factory for dotnet ef,
•	set provider-specific options (naming conventions, migrations assembly, history table),
•	register a small provider migrator implementation that the app can call to apply provider migrations at runtime.
Why this design
•	Keeps generated SQL/type mappings correct for each provider (no nvarchar in Postgres migrations).
•	Keeps migrations isolated and auditable per provider (separate folders/assemblies).
•	Keeps runtime app using the same domain IdDbContext while allowing a safe migrator to run provider migrations.
What we added (high level)
•	Two small provider projects:
•	Apps/ID/ID.Persistence.Ef.Postgres
•	Apps/ID/ID.Persistence.Ef.SQL
•	Provider migration context types (derive from the shared IdDbContext):
•	IdPostgresMigrationsContext
•	IdSqlServerMigrationsContext
•	Design-time factories so EF tooling can scaffold provider migrations:
•	IdPostgresDesignTimeFactory : IDesignTimeDbContextFactory<IdPostgresMigrationsContext>
•	IdSqlServerDesignTimeFactory : IDesignTimeDbContextFactory<IdSqlServerMigrationsContext>
•	Provider Setup.cs functions that return the DbContextOptionsBuilder configuration function used by the core setup:
•	PersistencePostgresSetup.GenerateConfigurationFunction(...)
•	(and same in SQL project)
•	These configure UseNpgsql / UseSqlServer, .UseSnakeCaseNamingConvention() for Postgres, .MigrationsHistoryTable(...), and set .MigrationsAssembly(...) when needed.
•	A small migrator abstraction and provider implementations:
•	ID.Persistence.Ef.Abstractions.IDbMigrator with Task MigrateAsync()
•	PostgresDbMigrator (in Postgres project) that calls migrationsDb.Database.MigrateAsync()
•	(SQL equivalent same pattern)
•	Wiring:
•	Core ID.Persistence.Setup.PersistenceSetup.AddPersistenceEf(...) accepts a provider Func<DbContextOptionsBuilder,...> so core wiring is provider‑agnostic.
•	Provider AddPersistenceEfPostgres(connectionString) registers provider migrator, calls services.AddPersistenceEf(...) and registers the migrations context IdPostgresMigrationsContext so the migrations assembly is loadable at runtime.
•	DbMntcService delegates migration to IDbMigrator (injected), so callers (controller/CLI) remain provider-agnostic.
Key files and responsibilities
•	Apps/ID/ID.Persistence\Setup\PersistenceSetup.cs
•	Core registration of IdDbContext using a passed configurationFunction.
•	Registers default IDbMntcService fallback; provider setups replace or inject migrator implementations.
•	Apps/ID/ID.Persistence\IdDbContext.cs
•	Shared domain DbContext and model configuration (provider-agnostic).
•	Important: provide an internal constructor that accepts non-generic DbContextOptions so EF tooling can pass DbContextOptions<TDerived> without cast errors.
•	Apps/ID/ID.Persistence.Ef.Postgres\Setup.cs
•	Exposes AddPersistenceEfPostgres(connectionString).
•	Provides GenerateConfigurationFunction that calls UseNpgsql(...), .MigrationsHistoryTable(...), .UseSnakeCaseNamingConvention().
•	Registers IdPostgresMigrationsContext in DI and registers PostgresDbMigrator as IDbMigrator.
•	Apps/ID/ID.Persistence.Ef.Postgres\IdPostgresMigrationsContext.cs
•	Small derived context: class IdPostgresMigrationsContext : IdDbContext.
•	Used only to scope Postgres migration types.
•	Apps/ID/ID.Persistence.Ef.Postgres\IdPostgresDesignTimeFactory.cs
•	Builds DbContextOptions<IdPostgresMigrationsContext> for EF tooling; EF uses this when you run dotnet ef migrations add ....
•	Apps/ID/ID.Persistence.Ef.Postgres\Migrations\*
•	Generated migrations and model snapshot are compiled into the Postgres provider assembly. They carry [DbContext(typeof(IdPostgresMigrationsContext))] metadata.
•	Apps/ID/ID.Persistence.Ef.Postgres\Services\PostgresDbMigrator.cs
•	Runs migrationsDb.Database.MigrateAsync() so runtime applies the migration types compiled into the provider assembly.
•	Apps/ID/ID.Presentation\Controllers\MaintenanceController.cs
•	Exposes protected admin action Migrate() which dispatches to a mediator (IdMigrateCmd) that ultimately calls IDbMntcService.MigrateAsync() which delegates to IDbMigrator.MigrateAsync().
How EF tooling and runtime tie together (summary)
1.	Add-Migration with --project pointing to provider project and --context IdPostgresMigrationsContext:
•	The design-time factory creates IdPostgresMigrationsContext configured for Npgsql + snake_case.
•	EF scaffolds migrations that are provider-specific and stores them in the provider project.
2.	At runtime:
•	IdPostgresMigrationsContext is registered in DI (provider Setup.cs) so the provider assembly and migration types are loadable.
•	PostgresDbMigrator resolves the migrations context and calls Database.GetPendingMigrations() / Database.MigrateAsync().
•	EF loads migrations from the migrations assembly and applies them to the DB, updating __EFMigrationsHistory in the configured history table/schema.
Commands you run
•	Design-time (create Postgres migration)
•	dotnet ef migrations add Initial_Postgres --context IdPostgresMigrationsContext --project Apps/ID/ID.Persistence.Ef.Postgres --startup-project Apps/ID/ID.Api -o Migrations/Postgres
•	Apply from CLI/runner:
•	dotnet ef database update --context IdPostgresMigrationsContext --project Apps/ID/ID.Persistence.Ef.Postgres --startup-project Apps/ID/ID.Api
•	Run programmatically (via admin/controller):
•	Call protected admin endpoint that triggers IDbMntcService.MigrateAsync() which delegates to provider IDbMigrator.
Important gotchas & lessons learned
•	DbContextOptions<T> casting problem
•	EF tooling constructs DbContextOptions<TDerived>. If IdDbContext only had a constructor DbContextOptions<IdDbContext>, passing DbContextOptions<IdPostgresMigrationsContext> fails at runtime.
•	Fix: add a non‑generic IdDbContext(DbContextOptions options) (internal) so derived construction works without casts.
•	Migrations assembly must be loadable at runtime
•	Ensure startup project has a ProjectReference to the provider migrations project or otherwise copies the compiled provider DLL into runtime output (your ID.Api.csproj includes such references).
•	Use MigrationsAssembly(...) and MigrationsHistoryTable(...) consistently
•	Set these in both design-time factory and runtime configuration so EF loads the right migrations and writes to the expected history table/schema.
•	Conventions vs explicit names
•	UseSnakeCaseNamingConvention() alters names globally. If any configuration uses .ToTable("Branches") or .HasColumnName("CreatedDate"), that explicit name wins; either make those explicit names snake_case or remove them to let the convention apply.
•	Where to apply migrations in CI/CD
•	Preferred: run migrations from CI or a deploy job (dotnet-ef or a one-off container job using the same image).
•	Fallback: secure admin endpoint in the app. If used, protect it (auth, token, env guard) and avoid automatic calls from public networks.
•	Avoid running migrations concurrently from multiple instances — use a lock (DB advisory lock or orchestration job).