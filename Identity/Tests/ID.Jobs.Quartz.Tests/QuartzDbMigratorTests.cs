//using ID.Application.Models;
//using ID.Jobs.Quartz.Persistence.Initializers;
//using Shouldly;

//namespace ID.Jobs.Quartz.Tests;

//public class QuartzDbMigratorTests
//{
//    [Fact]
//    public void EnsureSchema_InvokesSqlServerMigrator_WhenDatabaseTypeIsSqlServer()
//    {
//        bool called = false;
//        Persistence.Initializers.SqlServer.QuartzSqlServerMigrator.OnMigrateCalled = () => called = true;

//        QuartzDbMigrator.EnsureSchema(DatabaseType.SqlServer, "Server=.;Database=TestDb;User Id=sa;Password=Passw0rd!;", new NullLogger<QuartzDbMigrator>(), ensureDatabase: false);

//        called.ShouldBeTrue();

//        // cleanup
//        Persistence.Initializers.SqlServer.QuartzSqlServerMigrator.OnMigrateCalled = null;
//    }

//    //-----------------------//

//    [Fact]
//    public void EnsureSchema_InvokesPostgresMigrator_WhenDatabaseTypeIsPostgres()
//    {
//        bool called = false;
//        Persistence.Initializers.Postgres.QuartzPostgresServerMigrator.OnMigrateCalled = () => called = true;

//        QuartzDbMigrator.EnsureSchema(DatabaseType.PostgreSql, "Host=localhost;Database=TestDb;Username=test;Password=test;", new NullLogger<QuartzDbMigrator>(), ensureDatabase: false);

//        called.ShouldBeTrue();

//        // cleanup
//        Persistence.Initializers.Postgres.QuartzPostgresServerMigrator.OnMigrateCalled = null;
//    }

//}//Cls
