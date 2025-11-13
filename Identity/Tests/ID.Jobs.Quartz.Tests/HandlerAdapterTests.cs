using ID.Jobs.Quartz.AppImps.JobService;
using Quartz.Impl;

namespace ID.Jobs.Quartz.Tests;

public class HandlerAdapterTests
{
    private readonly ServiceProvider _sp;

    public HandlerAdapterTests()
    {
        var services = new ServiceCollection();
        // register a simple handler type for testing
        services.AddTransient<TestHandler>();
        _sp = services.BuildServiceProvider();
    }

    //----------------------//

    [Fact]
    public async Task Executes_TaskReturningMethod_NoParameters()
    {
        var adapter = new HandlerAdapter<TestHandler>(_sp, new NullLogger<HandlerAdapter<TestHandler>>());
        var ctx = new FakeJobExecutionContext("DoWork");

        await adapter.Execute(ctx);

        Assert.True(TestHandler.Worked);
    }

    //----------------------//

    [Fact]
    public async Task Executes_TaskReturningMethod_WithCancellationToken()
    {
        TestHandler.Worked = false;
        var adapter = new HandlerAdapter<TestHandler>(_sp, new NullLogger<HandlerAdapter<TestHandler>>());
        var ctx = new FakeJobExecutionContext("DoWorkWithToken");

        await adapter.Execute(ctx);

        Assert.True(TestHandler.Worked);
    }

    //----------------------//

    [Fact]
    public async Task UnsupportedSignature_Throws()
    {
        var adapter = new HandlerAdapter<TestHandler>(_sp, new NullLogger<HandlerAdapter<TestHandler>>());
        var ctx = new FakeJobExecutionContext("UnsupportedMethod");

        await Assert.ThrowsAsync<InvalidOperationException>(() => adapter.Execute(ctx));
    }

    [Fact]
    public async Task UnsupportedSignature_CachedThrowsOnSecondCall()
    {
        var adapter = new HandlerAdapter<TestHandler>(_sp, new NullLogger<HandlerAdapter<TestHandler>>());
        var ctx = new FakeJobExecutionContext("UnsupportedMethod");

        await Assert.ThrowsAsync<InvalidOperationException>(() => adapter.Execute(ctx));

        // second call should also throw (cached sentinel)
        await Assert.ThrowsAsync<InvalidOperationException>(() => adapter.Execute(ctx));
    }

    //######################//

    private class TestHandler
    {
        public static bool Worked { get; set; }

        public Task DoWork()
        {
            Worked = true;
            return Task.CompletedTask;
        }

        public Task DoWorkWithToken(CancellationToken ct)
        {
            Worked = true;
            return Task.CompletedTask;
        }

        public int UnsupportedMethod(string input)
        {
            return 42;
        }
    }

    //----------------------//

    // Minimal fake context to exercise JobDataMap retrieval
    private class FakeJobExecutionContext : IJobExecutionContext
    {
        private readonly JobDetailImpl _detail;
        private readonly JobDataMap _dataMap = new();

        public FakeJobExecutionContext(string methodName)
        {
            _detail = new JobDetailImpl("test", null, typeof(NoOpJob));
            _detail.JobDataMap.Put("MethodName", methodName);
        }

        // implement required members
        public IScheduler Scheduler => null!;
        public ITrigger Trigger => null!;
        public ICalendar Calendar => null!;
        public IJobDetail JobDetail => _detail;
        public IJobExecutionContext? PreviousFireTime => null;
        public CancellationToken CancellationToken => CancellationToken.None;
        public DateTimeOffset FireTimeUtc => default;
        public DateTimeOffset? ScheduledFireTimeUtc => default;
        public DateTimeOffset? NextFireTimeUtc => default;
        public DateTimeOffset? PreviousFireTimeUtc => default;
        public TimeSpan JobRunTime => default;
        public string FireInstanceId => string.Empty;
        public IJob JobInstance => new NoOpJob(); // return a minimal IJob
        public object? Result { get; set; }
        public JobDataMap MergedJobDataMap => _detail.JobDataMap;

        // IJobExecutionContext methods
        public void Put(object key, object value)
        {
            _detail.JobDataMap.Put(key.ToString() ?? string.Empty, value);
        }

        public object? Get(object key)
        {
            return _detail.JobDataMap.Get(key.ToString() ?? string.Empty);
        }

        public void Recover() { }
        public void SetResult(object? newResult) { Result = newResult; }
        public object? Remove(string key) { return null; }

        public bool Recovering => false;
        public TriggerKey? RecoveringTriggerKey => null;
        public int RefireCount => 0;
    }

    //----------------------//

    private class NoOpJob : IJob
    {
        public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
    }

}//Cls