//using ID.Jobs.Quartz.Persistence.Ef;
//using Microsoft.Data.SqlClient;
//using Moq;
//using System.Collections;
//using System.Data;
//using System.Data.Common;

//namespace ID.Jobs.Quartz.Tests;

//public class SqlEfCoreMigratorTests
//{
//    [Fact]
//    public void SplitBatches_ShouldSplitOnGoLines()
//    {
//        var script = "CREATE TABLE T1 (ID INT);\r\nGO\r\nCREATE TABLE T2 (ID INT);\r\nGO\r\n";
//        var batches = SqlEfCoreMigrator.SplitBatches(script);
//        batches.Length.ShouldBe(2);
//        batches[0].ShouldContain("CREATE TABLE T1");
//        batches[1].ShouldContain("CREATE TABLE T2");
//    }

//    //-----------------------//

//    [Fact]
//    public async Task MigrateAsync_SkipsAlreadyApplied_And_AppliesNewScripts()
//    {
//        // Arrange
//        var mockLoader = new Mock<IEmbeddedScriptLoader>();
//        mockLoader.Setup(l => l.LoadEmbeddedSqlScripts(It.IsAny<System.Reflection.Assembly>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>() ))
//            .Returns(
//            [
//                new QuartzSqlScript("001.sql", "CREATE TABLE T1 (ID INT);\r\nGO\r\nCREATE TABLE T2 (ID INT);\r\n")
//            ]);

//        var mockFactory = new Mock<IDbConnectionFactory<SqlConnection>>();

//        // Create a fake DbConnection that captures executed SQL
//        var fakeConn = new FakeDbConnection();
//        mockFactory.Setup(f => f.CreateConnection()).Returns(fakeConn as DbConnection);

//        var migrator = new SqlEfCoreMigrator(
//            new OptionsWrapper<QuartzConfig>(new QuartzConfig(DatabaseType.SqlServer, "fake")),
//            mockFactory.Object,
//            mockLoader.Object,
//            new NullLogger<SqlEfCoreMigrator>());

//        // Act
//        var result = await migrator.MigrateAsync(new Dictionary<string, string> { ["schema"] = "myid_qtz" });

//        // Assert
//        result.Succeeded.ShouldBeTrue();
//        result.AppliedScripts.Count.ShouldBe(1);
//        fakeConn.ExecutedCommands.Any(cmd => cmd.Contains("CREATE TABLE T1") || cmd.Contains("CREATE TABLE T2")).ShouldBeTrue();
//    }
//}

////############################################################################//

//// Simple fake DbConnection that returns a simple DbCommand capturing executed SQL
//internal class FakeDbConnection : DbConnection
//{
//    public List<string> ExecutedCommands { get; } = [];

//    public override string ConnectionString { get; set; } = "";

//    public override string Database => "FakeDb";
//    public override string DataSource => "Fake";
//    public override string ServerVersion => "1.0";
//    public override ConnectionState State => ConnectionState.Open;

//    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
//    public override void ChangeDatabase(string databaseName) => throw new NotSupportedException();
//    public override void Close() { }
//    public override void Open() { }
//    public override Task OpenAsync(CancellationToken cancellationToken) { return Task.CompletedTask; }

//    protected override DbCommand CreateDbCommand()
//    {
//        return new FakeDbCommand(this, ExecutedCommands);
//    }
//}

//internal class FakeDbCommand(FakeDbConnection conn, List<string> executed) : DbCommand
//{
//    public override string CommandText { get; set; }
//    public override int CommandTimeout { get; set; }
//    public override CommandType CommandType { get; set; }
//    private UpdateRowSource _updatedRowSource;
//    public override UpdateRowSource UpdatedRowSource { get => _updatedRowSource; set => _updatedRowSource = value; }

//    protected override DbConnection DbConnection { get => conn; set => throw new NotSupportedException(); }
//    protected override DbParameterCollection DbParameterCollection { get; } = new FakeDbParameterCollection();
//    protected override DbTransaction DbTransaction { get; set; }
//    public override bool DesignTimeVisible { get; set; }

//    public override void Cancel() { }
//    public override int ExecuteNonQuery() { executed.Add(CommandText); return 0; }
//    public override object ExecuteScalar() { executed.Add(CommandText); return null; }
//    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) { executed.Add(CommandText); return Task.FromResult(0); }
//    public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken) { executed.Add(CommandText); return Task.FromResult<object>(null); }

//    protected override DbParameter CreateDbParameter() => new FakeDbParameter();
//    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
//    public override void Prepare() { }
//}

//internal class FakeDbParameterCollection : DbParameterCollection
//{
//    private readonly List<DbParameter> _list = [];
//    public override int Add(object value) { _list.Add((DbParameter)value); return _list.Count - 1; }
//    public override void AddRange(Array values) { foreach (var v in values) _list.Add((DbParameter)v); }
//    public override void Clear() => _list.Clear();
//    public override bool Contains(object value) => _list.Contains((DbParameter)value);
//    public override bool Contains(string value) => _list.Any(p => p.ParameterName == value);
//    public override void CopyTo(Array array, int index) => _list.ToArray().CopyTo(array, index);
//    public override int Count => _list.Count;
//    public override IEnumerator GetEnumerator() => _list.GetEnumerator();
//    public override int IndexOf(object value) => _list.IndexOf((DbParameter)value);
//    public override int IndexOf(string parameterName) => _list.FindIndex(p => p.ParameterName == parameterName);
//    public override void Insert(int index, object value) => _list.Insert(index, (DbParameter)value);
//    public override void Remove(object value) => _list.Remove((DbParameter)value);
//    public override void RemoveAt(int index) => _list.RemoveAt(index);
//    public override void RemoveAt(string parameterName) => _list.RemoveAt(IndexOf(parameterName));
//    protected override DbParameter GetParameter(int index) => _list[index];
//    protected override DbParameter GetParameter(string parameterName) => _list.First(p => p.ParameterName == parameterName);
//    protected override void SetParameter(int index, DbParameter value) => _list[index] = value;
//    protected override void SetParameter(string parameterName, DbParameter value) { var i = IndexOf(parameterName); if (i >= 0) _list[i] = value; }
//    public override object SyncRoot => new();
//}

//internal class FakeDbParameter : DbParameter
//{
//    public override DbType DbType { get; set; }
//    public override ParameterDirection Direction { get; set; }
//    public override bool IsNullable { get; set; }
//    public override string ParameterName { get; set; }
//    public override string SourceColumn { get; set; }
//    public override object Value { get; set; }
//    public override bool SourceColumnNullMapping { get; set; }
//    public override int Size { get; set; }
//    public override void ResetDbType() { }
//}
