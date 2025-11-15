namespace ID.Jobs.Quartz.Persistence.Abs;



/// <summary>
/// Apply embedded SQL scripts (token-replaced) using EF Core-compatible execution.
/// Implementations must ensure idempotent application by using a journal table or EF migrations.
/// </summary>
internal interface IEfCoreMigrator
{
    /// <summary>
    /// Apply migrations. <paramref name="variables"/> contains token replacements (e.g. "schema").
    /// Returns a <see cref="QuartzMigrateResult"/> describing applied/skipped scripts and any error.
    /// </summary>
    Task<QuartzMigrateResult> MigrateAsync(Dictionary<string, string> variables, CancellationToken cancellationToken = default);
}