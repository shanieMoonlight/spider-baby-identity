using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ID.Jobs.Quartz.Persistence.DbUp.Postgres;
using ID.Jobs.Quartz.Persistence.Abs;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace ID.Jobs.Quartz.Tests.Migrators;

public class PgDbCommandExecutorTests
{
    [Fact]
    public async Task ExecuteNonQueryAndScalar_UsesConnectionAndParameters_ForPg()
    {
        // Arrange
        var fakeConn = new TestFakeDbConnection();
        int factoryCalls = 0;
        DbConnection factory() { factoryCalls++; return fakeConn; }

        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.PostgreSql, "fake-cs"));
        var executor = new PgDbCommandExecutor(opts, factory);

        // Act
        var nonQuerySql = "CREATE TABLE pg_table (id INT);";
        await executor.ExecuteNonQueryAsync(nonQuerySql, new Dictionary<string, object?> { ["p1"] = 123 });

        var scalarSql = "SELECT COUNT(1) FROM pg_table WHERE id = @p1;";
        fakeConn.ScalarResult = 42;
        var scalar = await executor.ExecuteScalarAsync(scalarSql, new Dictionary<string, object?> { ["p1"] = 123 });

        // Assert
        factoryCalls.ShouldBe(1);
        fakeConn.OpenCallCount.ShouldBe(1);
        fakeConn.ExecutedCommands.ShouldContain(cmd => cmd.Contains("CREATE TABLE pg_table"));
        fakeConn.ExecutedCommands.ShouldContain(cmd => cmd.Contains("SELECT COUNT(1)"));
        fakeConn.LastParameters.ShouldContainKey("@p1");
        fakeConn.LastParameters["@p1"].ShouldBe(123);
        ((int?)scalar).ShouldBe(42);
    }

    [Fact]
    public async Task EnsureOpenAsync_IsIdempotentUnderConcurrency_ForPg()
    {
        // Arrange
        var fakeConn = new TestFakeDbConnection();
        int factoryCalls = 0;
        DbConnection factory() { Interlocked.Increment(ref factoryCalls); return fakeConn; }

        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.PostgreSql, "fake-cs"));
        var executor = new PgDbCommandExecutor(opts, factory);

        // Act: call EnsureOpenAsync concurrently many times
        var tasks = Enumerable.Range(0, 10).Select(_ => executor.EnsureOpenAsync()).ToArray();
        await Task.WhenAll(tasks);

        // Assert
        factoryCalls.ShouldBe(1);
        fakeConn.OpenCallCount.ShouldBe(1);
    }
}
