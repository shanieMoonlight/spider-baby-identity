namespace ID.Jobs.Quartz.Tests.Migrators;

public class SqlDbCommandExecutorParameterTests
{
    [Fact]
    public async Task ParameterName_WithLeadingAt_IsPreserved()
    {
        var fakeConn = new TestFakeDbConnection();
        DbConnection factory() => fakeConn;
        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.SqlServer, "fake"));
        var executor = new SqlDbCommandExecutor(opts, factory);

        await executor.ExecuteNonQueryAsync("INSERT INTO T VALUES(@p)", new Dictionary<string, object?> { ["@p"] = 1 });

        fakeConn.LastParameters.ShouldContainKey("@p");
        fakeConn.LastParameters["@p"].ShouldBe(1);
    }

    //----------------------//

    [Fact]
    public async Task ParameterName_WithoutAt_IsPrefixedWithAt()
    {
        var fakeConn = new TestFakeDbConnection();
        DbConnection factory() => fakeConn;
        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.SqlServer, "fake"));
        var executor = new SqlDbCommandExecutor(opts, factory);

        await executor.ExecuteNonQueryAsync("INSERT INTO T VALUES(@p)", new Dictionary<string, object?> { ["p"] = 2 });

        fakeConn.LastParameters.ShouldContainKey("@p");
        fakeConn.LastParameters["@p"].ShouldBe(2);
    }

    //----------------------//

    [Fact]
    public async Task ParameterValue_Null_IsConvertedToDBNull()
    {
        var fakeConn = new TestFakeDbConnection();
        DbConnection factory() => fakeConn;
        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.SqlServer, "fake"));
        var executor = new SqlDbCommandExecutor(opts, factory);

        await executor.ExecuteNonQueryAsync("INSERT INTO T VALUES(@p)", new Dictionary<string, object?> { ["p"] = null });

        fakeConn.LastParameters.ShouldContainKey("@p");
        fakeConn.LastParameters["@p"].ShouldBeNull();
    }

    //----------------------//

    [Fact]
    public async Task ParameterName_EmptyKey_ThrowsArgumentException()
    {
        var fakeConn = new TestFakeDbConnection();
        DbConnection factory() => fakeConn;
        var opts = new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.SqlServer, "fake"));
        var executor = new SqlDbCommandExecutor(opts, factory);

        await Should.ThrowAsync<ArgumentException>(async () =>
        {
            await executor.ExecuteNonQueryAsync("INSERT INTO T VALUES(@p)", new Dictionary<string, object?> { [" "] = 1 });
        });
    }
}
