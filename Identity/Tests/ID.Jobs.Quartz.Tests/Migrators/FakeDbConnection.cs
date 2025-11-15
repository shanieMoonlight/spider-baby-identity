namespace ID.Jobs.Quartz.Tests.Migrators;


// Simple test fake DbConnection and command to capture executed SQL and parameters
internal class TestFakeDbConnection : DbConnection
{
    public List<string> ExecutedCommands { get; } = [];
    public Dictionary<string, object?> LastParameters { get; } = [];
    public int OpenCallCount { get; private set; }
    public object? ScalarResult { get; set; } = null;

    private ConnectionState _state = ConnectionState.Closed;

    [DefaultValue("")]
    [AllowNull]
    public override string ConnectionString { get; set; } = string.Empty;
    public override string Database => "FakeDb";
    public override string DataSource => "Fake";
    public override string ServerVersion => "1.0";
    public override ConnectionState State => _state;

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
    public override void ChangeDatabase(string databaseName) => throw new NotSupportedException();
    public override void Close() { _state = ConnectionState.Closed; }
    public override void Open() { OpenCallCount++; _state = ConnectionState.Open; }
    public override Task OpenAsync(CancellationToken cancellationToken) { OpenCallCount++; _state = ConnectionState.Open; return Task.CompletedTask; }

    protected override DbCommand CreateDbCommand() => new TestFakeDbCommand(this);

    internal void CaptureExecution(string sql, IEnumerable<DbParameter>? parameters)
    {
        ExecutedCommands.Add(sql);
        LastParameters.Clear();
        if (parameters != null)
        {
            foreach (DbParameter p in parameters)
            {
                LastParameters[p.ParameterName] = p.Value == DBNull.Value ? null : p.Value;
            }
        }
    }
}

internal class TestFakeDbCommand(TestFakeDbConnection conn) : DbCommand
{
    private readonly List<DbParameter> _parameters = [];

    [DefaultValue("")]
    [AllowNull]
    public override string CommandText { get; set; } = string.Empty;
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }
    public override UpdateRowSource UpdatedRowSource { get; set; }

    protected override DbConnection? DbConnection { get => conn; set => throw new NotSupportedException(); }
    protected override DbParameterCollection DbParameterCollection { get; } = new TestFakeDbParameterCollection();

    protected override DbTransaction? DbTransaction { get; set; }
    public override bool DesignTimeVisible { get; set; }

    public override void Cancel() { }
    public override int ExecuteNonQuery() { conn.CaptureExecution(CommandText, _parameters); return 1; }
    public override object ExecuteScalar() { conn.CaptureExecution(CommandText, _parameters); return conn.ScalarResult ?? 0; }
    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) { conn.CaptureExecution(CommandText, _parameters); return Task.FromResult(1); }
    public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken) { conn.CaptureExecution(CommandText, _parameters); return Task.FromResult(conn.ScalarResult ?? 0); }

    protected override DbParameter CreateDbParameter()
    {
        var p = new TestFakeDbParameter();
        _parameters.Add(p);
        return p;
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
    public override void Prepare() { }
}

internal class TestFakeDbParameterCollection : DbParameterCollection
{
    private readonly List<DbParameter> _list = [];
    public override int Add(object value) { _list.Add((DbParameter)value); return _list.Count - 1; }
    public override void AddRange(Array values) { foreach (var v in values) _list.Add((DbParameter)v); }
    public override void Clear() => _list.Clear();
    public override bool Contains(object value) => _list.Contains((DbParameter)value);
    public override bool Contains(string value) => _list.Any(p => p.ParameterName == value);
    public override void CopyTo(Array array, int index) => _list.ToArray().CopyTo(array, index);
    public override int Count => _list.Count;
    public override IEnumerator GetEnumerator() => _list.GetEnumerator();
    public override int IndexOf(object value) => _list.IndexOf((DbParameter)value);
    public override int IndexOf(string parameterName) => _list.FindIndex(p => p.ParameterName == parameterName);
    public override void Insert(int index, object value) => _list.Insert(index, (DbParameter)value);
    public override void Remove(object value) => _list.Remove((DbParameter)value);
    public override void RemoveAt(int index) => _list.RemoveAt(index);
    public override void RemoveAt(string parameterName) => _list.RemoveAt(IndexOf(parameterName));
    protected override DbParameter GetParameter(int index) => _list[index];
    protected override DbParameter GetParameter(string parameterName) => _list.First(p => p.ParameterName == parameterName);
    protected override void SetParameter(int index, DbParameter value) => _list[index] = value;
    protected override void SetParameter(string parameterName, DbParameter value) { var i = IndexOf(parameterName); if (i >= 0) _list[i] = value; }
    public override object SyncRoot => new();
}

internal class TestFakeDbParameter : DbParameter
{
    public override DbType DbType { get; set; }
    public override ParameterDirection Direction { get; set; }
    public override bool IsNullable { get; set; }

    [DefaultValue("")]
    [AllowNull]
    public override string ParameterName { get; set; }

    [DefaultValue("")]
    [AllowNull]
    public override string SourceColumn { get; set; }

    [DefaultValue("")]
    [AllowNull]
    public override object Value { get; set; }
    public override bool SourceColumnNullMapping { get; set; }
    public override int Size { get; set; }
    public override void ResetDbType() { }
}
