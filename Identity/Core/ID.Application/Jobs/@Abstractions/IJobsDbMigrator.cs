namespace ID.Application.Jobs.Abstractions;

public interface IJobsDbMigrator
{
    Task MigrateAsync(CancellationToken cancellationToken);
}