namespace ID.Jobs.Quartz.Tests.Migrators;

public class SqlDbCommandExecutorTests
{
    [Fact]
    public async Task ExecuteNonQueryAndScalar_UsesConnectionAndParameters()
    {
        // Arrange
        var fakeConn = new TestFakeDbConnection();
        int factoryCalls = 0;
        DbConnection factory() { factoryCalls++; return fakeConn; }

        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.SqlServer, "fake-cs"));
        var executor = new SqlDbCommandExecutor(opts, factory);

        // Act
        var nonQuerySql = "INSERT INTO T(A) VALUES (@p1);";
        await executor.ExecuteNonQueryAsync(nonQuerySql, new Dictionary<string, object?> { ["p1"] = 123 });

        var scalarSql = "SELECT COUNT(1) FROM T WHERE A = @p1;";
        fakeConn.ScalarResult = 7;
        var scalar = await executor.ExecuteScalarAsync(scalarSql, new Dictionary<string, object?> { ["p1"] = 123 });

        // Assert
        factoryCalls.ShouldBe(1);
        fakeConn.OpenCallCount.ShouldBe(1);
        fakeConn.ExecutedCommands.ShouldContain(cmd => cmd.Contains("INSERT INTO T"));
        fakeConn.ExecutedCommands.ShouldContain(cmd => cmd.Contains("SELECT COUNT(1)"));
        fakeConn.LastParameters.ShouldContainKey("@p1");
        fakeConn.LastParameters["@p1"].ShouldBe(123);
        ((int?)scalar).ShouldBe(7);
    }

    //----------------------//

    [Fact]
    public async Task EnsureOpenAsync_IsIdempotentUnderConcurrency()
    {
        // Arrange
        var fakeConn = new TestFakeDbConnection();
        int factoryCalls = 0;
        DbConnection factory() { Interlocked.Increment(ref factoryCalls); return fakeConn; }

        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.SqlServer, "fake-cs"));
        var executor = new SqlDbCommandExecutor(opts, factory);

        // Act: call EnsureOpenAsync concurrently many times
        var tasks = Enumerable.Range(0, 10).Select(_ => executor.EnsureOpenAsync()).ToArray();
        await Task.WhenAll(tasks);

        // Assert
        factoryCalls.ShouldBe(1);
        fakeConn.OpenCallCount.ShouldBe(1);
    }
}
