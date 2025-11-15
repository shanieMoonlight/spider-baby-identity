//namespace ID.Jobs.Quartz.Tests;

//public class SetupPersistenceTests
//{
//    [Fact]
//    public void AddQuartzPersistence_RegistersSqlServerMigrator_WhenDatabaseTypeIsSqlServer()
//    {
//        var services = new ServiceCollection();

//        services.AddQuartzPersistence(DatabaseType.SqlServer);

//        var sd = services.FirstOrDefault(s => s.ServiceType == typeof(IDbUpMigrator));
//        sd.ShouldNotBeNull();
//        sd.Lifetime.ShouldBe(ServiceLifetime.Scoped);
//        sd.ImplementationType.ShouldBe(typeof(DbUpSqlServerMigrator));
//    }

//    //-----------------------//

//    [Fact]
//    public void AddQuartzPersistence_RegistersPostgresMigrator_WhenDatabaseTypeIsPostgres()
//    {
//        var services = new ServiceCollection();

//        services.AddQuartzPersistence(DatabaseType.PostgreSql);

//        var sd = services.FirstOrDefault(s => s.ServiceType == typeof(IDbUpMigrator));
//        sd.ShouldNotBeNull();
//        sd.Lifetime.ShouldBe(ServiceLifetime.Scoped);
//        sd.ImplementationType.ShouldBe(typeof(DbUpPostgresServerMigrator));
//    }

//}//Cls
