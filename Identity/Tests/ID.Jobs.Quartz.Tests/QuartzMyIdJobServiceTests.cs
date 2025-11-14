using ID.Application.Jobs.Abstractions;
using ID.Jobs.Quartz.AppImps.JobService;
using ID.Jobs.Quartz.Retries;
using Moq;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace ID.Jobs.Quartz.Tests;

public class QuartzMyIdJobServiceTests
{
    [Fact]
    public async Task StartRecurringJob_WhenSchedulerUnavailable_StoresPendingRetry()
    {
        // Arrange
        var mockFactory = new Mock<ISchedulerFactory>();
        mockFactory.Setup(f => f.GetScheduler(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("DB not ready"));

        var store = new PendingRetryStore();

        var svc = new QuartzMyIdJobService(mockFactory.Object, new NullLogger<QuartzMyIdJobService>(), store);

        var jobId = "test.job";

        // Act
        var result = await svc.StartRecurringJob<TestHandler>(jobId, h => h.HandleAsync(), "0 0/5 * * * ?");

        // Assert
        result.ShouldBeFalse();
        store.TryGet(jobId, out var pending).ShouldBeTrue();
        pending.ShouldNotBeNull();
        pending.Description.ShouldBe($"StartRecurringJob {jobId}");
    }

    //-----------------------//

    [Fact]
    public async Task StartRecurringJob_Success_SchedulesAndReturnsTrue()
    {

        var mockFactory = new Mock<ISchedulerFactory>();
        var mockScheduler = new Mock<IScheduler>();

        mockScheduler.Setup(s => s.AddJob(It.IsAny<IJobDetail>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mockScheduler.Setup(s => s.GetTriggersOfJob(It.IsAny<JobKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ITrigger>());
        // ScheduleJob may return Task<DateTimeOffset?> or Task - cover common case by returning completed task
        mockScheduler.Setup(s => s.ScheduleJob(It.IsAny<ITrigger>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DateTimeOffset.UtcNow));

        mockFactory.Setup(f => f.GetScheduler(It.IsAny<CancellationToken>())).ReturnsAsync(mockScheduler.Object);

        var store = new PendingRetryStore();
        var svc = new QuartzMyIdJobService(mockFactory.Object, new NullLogger<QuartzMyIdJobService>(), store);

        var jobId = "success.job";

        var result = await svc.StartRecurringJob<TestHandler>(jobId, h => h.HandleAsync(), "0 0/5 * * * ?");

        result.ShouldBeTrue();
        store.TryGet(jobId, out _).ShouldBeFalse();
        mockScheduler.Verify(s => s.AddJob(It.IsAny<IJobDetail>(), true, It.IsAny<CancellationToken>()), Times.Once);
        mockScheduler.Verify(s => s.ScheduleJob(It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    //-----------------------//

    [Fact]
    public async Task StopRecurringJob_CallsDeleteJobAndReturnsResult()
    {
        var mockFactory = new Mock<ISchedulerFactory>();
        var mockScheduler = new Mock<IScheduler>();

        mockScheduler.Setup(s => s.DeleteJob(It.IsAny<JobKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        mockFactory.Setup(f => f.GetScheduler(It.IsAny<CancellationToken>())).ReturnsAsync(mockScheduler.Object);

        var svc = new QuartzMyIdJobService(mockFactory.Object, new NullLogger<QuartzMyIdJobService>(), new PendingRetryStore());

        var result = await svc.StopRecurringJob("some.job");

        result.ShouldBeTrue();
        mockScheduler.Verify(s => s.DeleteJob(It.Is<JobKey>(k => k.Name == "some.job" && k.Group == QuartzConstants.JobGroup), It.IsAny<CancellationToken>()), Times.Once);
    }

    //-----------------------//

    [Fact]
    public async Task GetRecurringJobsAsync_ReturnsJobs()
    {
        var mockFactory = new Mock<ISchedulerFactory>();
        var mockScheduler = new Mock<IScheduler>();

        var jk = new JobKey("j1", QuartzConstants.JobGroup);
        var jobKeys = new HashSet<JobKey> { jk };

        var jobDetail = new JobDetailImpl(jk.Name, jk.Group, typeof(NoOpJob));
        jobDetail.JobDataMap.Put(QuartzConstants.HandlerTypeKey, "MyHandlerType");
        jobDetail.JobDataMap.Put(QuartzConstants.MethodNameKey, "HandleAsync");

        var cronTrigger = Mock.Of<ICronTrigger>(t => t.CronExpressionString == "0 0/5 * * * ?");

        mockScheduler.Setup(s => s.GetJobKeys(It.IsAny<GroupMatcher<JobKey>>(), It.IsAny<CancellationToken>())).ReturnsAsync(jobKeys);
        mockScheduler.Setup(s => s.GetJobDetail(jk, It.IsAny<CancellationToken>())).ReturnsAsync(jobDetail);
        mockScheduler.Setup(s => s.GetTriggersOfJob(jk, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ITrigger> { cronTrigger });

        mockFactory.Setup(f => f.GetScheduler(It.IsAny<CancellationToken>())).ReturnsAsync(mockScheduler.Object);

        var svc = new QuartzMyIdJobService(mockFactory.Object, new NullLogger<QuartzMyIdJobService>(), new PendingRetryStore());

        var result = await svc.GetRecurringJobsAsync();

        result.Count.ShouldBe(1);
        var item = result[0];
        item.Id.ShouldBe("j1");
        item.Cron.ShouldBe("0 0/5 * * * ?");
        item.Job.Method.ShouldBe("HandleAsync");
        item.Job.Type.ShouldBe("MyHandlerType");
    }

    //-----------------------//

    private class TestHandler : AMyIdJobHandler
    {
        public TestHandler() : base("TEST_HANDLER") { }
        public override Task HandleAsync() => Task.CompletedTask;
    }

    private class NoOpJob : IJob { public Task Execute(IJobExecutionContext context) => Task.CompletedTask; }

}//Cls
