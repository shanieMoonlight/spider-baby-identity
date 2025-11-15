using ID.Jobs.Quartz.Persistence.Ef.SqlServer;

namespace ID.Jobs.Quartz.Tests;

public class SetupPersistenceTests
{
    [Fact]
    public void AddQuartzPersistence_RegistersSqlServerServices_WhenDatabaseTypeIsSqlServer()
    {
        var services = new ServiceCollection();

        services.AddQuartzPersistence(DatabaseType.SqlServer);

        var executor = services.FirstOrDefault(s => s.ServiceType == typeof(IDbCommandExecutor));
        var migrator = services.FirstOrDefault(s => s.ServiceType == typeof(IEfCoreMigrator));


        executor.ShouldNotBeNull();
        executor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        executor.ImplementationType.ShouldBe(typeof(SqlDbCommandExecutor));

        migrator.ShouldNotBeNull();
        migrator.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        migrator.ImplementationType.ShouldBe(typeof(SqlEfCoreMigrator));
    }

    //-----------------------//

    [Fact]
    public void AddQuartzPersistence_RegistersPostgresServices_WhenDatabaseTypeIsPostgres()
    {
        var services = new ServiceCollection();

        services.AddQuartzPersistence(DatabaseType.PostgreSql);

        var executor = services.FirstOrDefault(s => s.ServiceType == typeof(IDbCommandExecutor));
        var migrator = services.FirstOrDefault(s => s.ServiceType == typeof(IEfCoreMigrator));


        executor.ShouldNotBeNull();
        executor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        executor.ImplementationType.ShouldBe(typeof(PgDbCommandExecutor));

        migrator.ShouldNotBeNull();
        migrator.Lifetime.ShouldBe(ServiceLifetime.Scoped);
        migrator.ImplementationType.ShouldBe(typeof(PgEfCoreMigrator));
    }

}//Cls
