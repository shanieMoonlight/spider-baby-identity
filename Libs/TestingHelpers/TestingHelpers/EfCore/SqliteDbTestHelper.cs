using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TestingHelpers.EfCore;

public static class SqliteDbTestHelper
{

    public static void TestIt<Ctx>(
        Func<DbContextOptions<Ctx>, Ctx> contextFactory,
        Action<Ctx> populateDbLmda,
        Action<Ctx> testLmda)
        where Ctx : DbContext
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();


        var options = new DbContextOptionsBuilder<Ctx>()
            .UseSqlite(connection)
  
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // avoid fix-up
            .Options;



        using (var context = contextFactory(options))
        {
            context.Database.EnsureCreated();

            populateDbLmda(context);

            context.SaveChanges();
        }


        // Act
        using (var context = contextFactory(options))
        {
            testLmda(context);
        }
    }

}//Cls
