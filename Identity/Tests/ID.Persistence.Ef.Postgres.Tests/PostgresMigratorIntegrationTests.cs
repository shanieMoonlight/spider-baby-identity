//using ID.Persistence.Ef.Postgres.Services;
//using Microsoft.Data.Sqlite;
//using Microsoft.EntityFrameworkCore;
//using Shouldly;

//namespace ID.Persistence.Ef.Postgres.Tests;

//TODO: Uncomment this and do it properly when we have Postgres test setup

//public class PostgresMigratorIntegrationTests
//{
//    [Fact]
//    public async Task PostgresMigrator_MigrateAsync_Completes_WithSqliteInMemory()
//    {
//        // use a shared in-memory Sqlite connection so EF in-memory DB persists across context instances
//        var connection = new SqliteConnection("DataSource=:memory:");
//        await connection.OpenAsync();

//        var options = new DbContextOptionsBuilder<IdPostgresMigrationsContext>()
//            .UseSqlite(connection)
//            .Options;

//        await using (var context = new IdPostgresMigrationsContext(options))
//        {
//            // ensure migrations assembly applied (this will create the database schema if migrations exist)
//            var migrator = new PostgresDbMigrator(context);
//            await migrator.MigrateAsync();
//        }

//        // If we reached here without exception the migrator completed.
//        connection.State.ShouldBe(System.Data.ConnectionState.Open);

//        await connection.CloseAsync();
//    }
//}
