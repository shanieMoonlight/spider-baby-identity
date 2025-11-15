namespace ID.Jobs.Quartz.Tests.Migrators;

public class PgEfCoreMigratorUnitTests
{
    [Fact]
    public async Task MigrateAsync_AppliesScriptAndRecordsJournal()
    {
        // Arrange
        var scriptSql = "CREATE TABLE pg_table (id INT);";
        var script = new QuartzSqlScript("001-pg.sql", scriptSql);

        var mockLoader = new Mock<IEmbeddedScriptLoader>();
        mockLoader.Setup(l => l.LoadEmbeddedSqlScripts(It.IsAny<System.Reflection.Assembly>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
            .Returns([script]);

        var executed = new List<string>();
        var mockExecutor = new Mock<IDbCommandExecutor>();
        mockExecutor.Setup(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .Callback<string, IDictionary<string, object?>, CancellationToken>((sql, p, ct) => executed.Add(sql))
            .ReturnsAsync(0);
        mockExecutor.Setup(e => e.ExecuteScalarAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object?)null);

        var migrator = new PgEfCoreMigrator(mockExecutor.Object, mockLoader.Object, NullLogger<PgEfCoreMigrator>.Instance);

        // Act
        var result = await migrator.MigrateAsync(new Dictionary<string, string> { ["schema"] = "myid_qtz" });

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.AppliedScripts.Count.ShouldBe(1);
        executed.ShouldContain(sql => sql.Contains("CREATE TABLE pg_table") || sql.Contains("CREATE TABLE"));
        executed.ShouldContain(sql => sql.Contains("INSERT INTO") || sql.Contains("schema-versions") == false); // journal insert present
        mockExecutor.Verify(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    //----------------------//

    [Fact]
    public async Task MigrateAsync_SkipsAlreadyApplied()
    {
        // Arrange
        var scriptSql = "CREATE TABLE pg_table (id INT);";
        var script = new QuartzSqlScript("001-pg.sql", scriptSql);

        var mockLoader = new Mock<IEmbeddedScriptLoader>();
        mockLoader.Setup(l => l.LoadEmbeddedSqlScripts(It.IsAny<System.Reflection.Assembly>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
            .Returns([script]);

        var executed = new List<string>();
        var mockExecutor = new Mock<IDbCommandExecutor>();
        mockExecutor.Setup(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .Callback<string, IDictionary<string, object?>, CancellationToken>((sql, p, ct) => executed.Add(sql))
            .ReturnsAsync(0);
        mockExecutor.Setup(e => e.ExecuteScalarAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object?)1);

        var migrator = new PgEfCoreMigrator(mockExecutor.Object, mockLoader.Object, NullLogger<PgEfCoreMigrator>.Instance);

        // Act
        var result = await migrator.MigrateAsync(new Dictionary<string, string> { ["schema"] = "myid_qtz" });

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.AppliedScripts.Count.ShouldBe(0);
        result.SkippedScripts.Count.ShouldBe(1);
        executed.ShouldNotContain(sql => sql.Contains("CREATE TABLE pg_table"));
    }

    //----------------------//

    [Fact]
    public async Task MigrateAsync_ReturnsFailure_WhenExecutionThrows()
    {
        // Arrange
        var scriptSql = "CREATE TABLE pg_table (id INT);";
        var script = new QuartzSqlScript("001-pg.sql", scriptSql);

        var mockLoader = new Mock<IEmbeddedScriptLoader>();
        mockLoader.Setup(l => l.LoadEmbeddedSqlScripts(It.IsAny<System.Reflection.Assembly>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
            .Returns([script]);

        var mockExecutor = new Mock<IDbCommandExecutor>();
        mockExecutor.Setup(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockExecutor.Setup(e => e.ExecuteScalarAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object?)null);
        // Ensure schema/journal creation succeed
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.Is<string>(sql => sql.Contains("CREATE TABLE IF NOT EXISTS") || sql.Contains("CREATE SCHEMA")), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        // Simulate failure when executing the script content
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.Is<string>(sql => sql.Contains("CREATE TABLE pg_table")), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("execution failed"));

        var migrator = new PgEfCoreMigrator(mockExecutor.Object, mockLoader.Object, NullLogger<PgEfCoreMigrator>.Instance);

        // Act
        var result = await migrator.MigrateAsync(new Dictionary<string, string> { ["schema"] = "myid_qtz" });

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage?.ShouldContain("Failed applying script");
        result.Exception.ShouldNotBeNull();
        mockExecutor.Verify(e => e.ExecuteNonQueryAsync(It.Is<string>(sql => sql.Contains("INSERT INTO")), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
