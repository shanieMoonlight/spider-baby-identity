using ID.Application.Jobs.Abstractions;
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
        services.AddTransient<FailingHandler>();
        _sp = services.BuildServiceProvider();
    }

    //----------------------//

    [Fact]
    public async Task Execute_Invokes_HandleAsync()
    {
        TestHandler.Worked = false;
        var adapter = new HandlerAdapter<TestHandler>(_sp, new NullLogger<HandlerAdapter<TestHandler>>());
        var ctx = new FakeJobExecutionContext();

        await adapter.Execute(ctx);

        Assert.True(TestHandler.Worked);
    }

    //----------------------//

    [Fact]
    public async Task Execute_HandlerThrows_ExceptionPropagates()
    {
        var adapter = new HandlerAdapter<FailingHandler>(_sp, new NullLogger<HandlerAdapter<FailingHandler>>());
        var ctx = new FakeJobExecutionContext();

        await Assert.ThrowsAsync<InvalidOperationException>(() => adapter.Execute(ctx));
    }

    //######################//

    private class TestHandler : AMyIdJobHandler
    {
        public static bool Worked { get; set; }

        public TestHandler() : base("TEST_HANDLER") { }

        public override Task HandleAsync()
        {
            Worked = true;
            return Task.CompletedTask;
        }
    }

    private class FailingHandler : AMyIdJobHandler
    {
        public FailingHandler() : base("FAIL_HANDLER") { }

        public override Task HandleAsync() => throw new InvalidOperationException("handler failed");
    }

    //----------------------//

    // Minimal fake context to exercise JobDataMap retrieval
    private class FakeJobExecutionContext : IJobExecutionContext
    {
        private readonly JobDetailImpl _detail;

        public FakeJobExecutionContext()
        {
            _detail = new JobDetailImpl("test", null!, typeof(NoOpJob));
        }

        // implement required members
        public IScheduler Scheduler => null!;
        public ITrigger Trigger => null!;
        public ICalendar Calendar => null!;
        public IJobDetail JobDetail => _detail;
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

        public void SetResult(object? newResult) { Result = newResult; }

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