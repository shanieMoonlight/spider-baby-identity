using ID.Jobs.Quartz.Persistence.Ef;
using Moq;

namespace ID.Jobs.Quartz.Tests;

public class SqlEfCoreMigratorUnitTests
{
    [Fact]
    public async Task MigrateAsync_AppliesBatchesAndRecordsJournal()
    {
        // Arrange
        var scriptSql = "CREATE TABLE T1 (ID INT);\r\nGO\r\nCREATE TABLE T2 (ID INT);\r\n";
        var script = new QuartzSqlScript("001-create.sql", scriptSql);

        var mockLoader = new Mock<IEmbeddedScriptLoader>();
        mockLoader.Setup(l => l.LoadEmbeddedSqlScripts(It.IsAny<System.Reflection.Assembly>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
            .Returns([script]);

        var executed = new List<string>();
        var mockExecutor = new Mock<IDbCommandExecutor>();
        mockExecutor.Setup(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        // Schema/journal creation and batch execution all use ExecuteNonQueryAsync - capture SQL
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .Callback<string, IDictionary<string, object?>, CancellationToken>((sql, p, ct) => executed.Add(sql))
            .ReturnsAsync(0);
        // Check journal (ExecuteScalar) should return null => not applied
        mockExecutor.Setup(e => e.ExecuteScalarAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object?)null);

        var migrator = new SqlEfCoreMigrator(mockExecutor.Object, mockLoader.Object, NullLogger<SqlEfCoreMigrator>.Instance);

        // Act
        var result = await migrator.MigrateAsync(new Dictionary<string, string> { ["schema"] = "myid_qtz" });

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.AppliedScripts.Count.ShouldBe(1);
        // Expect both create table batches executed
        executed.ShouldContain(sql => sql.Contains("CREATE TABLE T1"));
        executed.ShouldContain(sql => sql.Contains("CREATE TABLE T2"));
        // Expect a journal INSERT executed
        executed.ShouldContain(sql => sql.Contains("INSERT INTO") || sql.Contains("SchemaVersions") || sql.Contains("SchemaVersions"));
        mockExecutor.Verify(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    //----------------------//

    [Fact]
    public async Task MigrateAsync_SkipsAlreadyApplied()
    {
        // Arrange
        var scriptSql = "CREATE TABLE T1 (ID INT);\r\nGO\r\n";
        var script = new QuartzSqlScript("001-create.sql", scriptSql);

        var mockLoader = new Mock<IEmbeddedScriptLoader>();
        mockLoader.Setup(l => l.LoadEmbeddedSqlScripts(It.IsAny<System.Reflection.Assembly>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
            .Returns([script]);

        var executed = new List<string>();
        var mockExecutor = new Mock<IDbCommandExecutor>();
        mockExecutor.Setup(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .Callback<string, IDictionary<string, object?>, CancellationToken>((sql, p, ct) => executed.Add(sql))
            .ReturnsAsync(0);
        // Simulate journal reports script already applied
        mockExecutor.Setup(e => e.ExecuteScalarAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object?)1);

        var migrator = new SqlEfCoreMigrator(mockExecutor.Object, mockLoader.Object, NullLogger<SqlEfCoreMigrator>.Instance);

        // Act
        var result = await migrator.MigrateAsync(new Dictionary<string, string> { ["schema"] = "myid_qtz" });

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.AppliedScripts.Count.ShouldBe(0);
        result.SkippedScripts.Count.ShouldBe(1);
        // No script batch executed (only schema/journal creation may have run) - ensure no CREATE TABLE T1 executed
        executed.ShouldNotContain(sql => sql.Contains("CREATE TABLE T1"));
    }

    [Fact]
    public async Task MigrateAsync_ReturnsFailure_WhenBatchThrows()
    {
        // Arrange
        var scriptSql = "CREATE TABLE T1 (ID INT);\r\nGO\r\nCREATE TABLE T2 (ID INT);\r\n";
        var script = new QuartzSqlScript("001-create.sql", scriptSql);

        var mockLoader = new Mock<IEmbeddedScriptLoader>();
        mockLoader.Setup(l => l.LoadEmbeddedSqlScripts(It.IsAny<System.Reflection.Assembly>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
            .Returns([script]);

        var mockExecutor = new Mock<IDbCommandExecutor>();
        mockExecutor.Setup(e => e.EnsureOpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        // Check journal returns null (not applied)
        mockExecutor.Setup(e => e.ExecuteScalarAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((object?)null);
        // Schema/journal creation succeed
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.Is<string>(sql => sql.Contains("IF NOT EXISTS") || sql.Contains("CREATE TABLE")), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        // Simulate batch failure when executing CREATE TABLE T1
        mockExecutor.Setup(e => e.ExecuteNonQueryAsync(It.Is<string>(sql => sql.Contains("CREATE TABLE T1")), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception("batch failed"));

        var migrator = new SqlEfCoreMigrator(mockExecutor.Object, mockLoader.Object, NullLogger<SqlEfCoreMigrator>.Instance);

        // Act
        var result = await migrator.MigrateAsync(new Dictionary<string, string> { ["schema"] = "myid_qtz" });

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage?.ShouldContain("Failed applying script");
        result.Exception.ShouldNotBeNull();
        // Ensure journal insert was not attempted
        mockExecutor.Verify(e => e.ExecuteNonQueryAsync(It.Is<string>(sql => sql.Contains("INSERT INTO")), It.IsAny<IDictionary<string, object?>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
