using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using ID.Application.Jobs.Abstractions;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Models;
using Moq;
using System.Linq.Expressions;

namespace ID.Infrastructure.Tests.Jobs.Service.HF.Recurring;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.


/// <summary>
/// Base test class for recurring job manager implementations
/// </summary>
/// <typeparam name="TManager">The concrete manager type to test</typeparam>
public abstract class AHfRecurringJobMgrTests
   
{
    internal readonly Mock<IHangfireInstanceProvider> MockInstanceProvider;
    internal readonly Mock<IRecurringJobManagerWrapper> MockRecurringJobManager;
    internal readonly IHfRecurringJobMgr JobManager;
    protected readonly string ExpectedQueue;

    protected AHfRecurringJobMgrTests(string expectedQueue)
    {
        ExpectedQueue = expectedQueue;

        // Setup mocks
        MockRecurringJobManager = new Mock<IRecurringJobManagerWrapper>();

        MockInstanceProvider = new Mock<IHangfireInstanceProvider>();
        MockInstanceProvider
            .Setup(x => x.RecurringJobManager)
            .Returns(MockRecurringJobManager.Object);

        // Create the concrete manager through the factory method
        JobManager = CreateJobManager(MockInstanceProvider.Object);
    }

    //########################//

    /// <summary>
    /// Factory method to create the concrete manager implementation
    /// </summary>
    internal abstract IHfRecurringJobMgr CreateJobManager(IHangfireInstanceProvider instanceProvider);


    //- - - - - - - - - - - - //


    [Fact]
    public abstract void JobManager_Should_ImplementCorrectInterface();

    //########################//

    [Fact]
    public void AddOrUpdate_Should_UseCorrectQueue()
    {
        // Arrange
        var recurringJobId = "recurring-job-123";
        var cronExpression = "0 0 * * *"; // Daily at midnight
        Expression<Func<TestJobHandler, Task>> jobExpression = j => j.DoWorkAsync();
        var options = new HfRecurringJobOptions();
        RecurringJobOptions recurringJobOptions = null;

        MockRecurringJobManager
            .Setup(x => x.AddOrUpdate(
                recurringJobId,
                ExpectedQueue,
                jobExpression,
                cronExpression,
                It.IsAny<RecurringJobOptions>()))
            .Callback<string, string, Expression<Func<TestJobHandler, Task>>, string, RecurringJobOptions>(
                (id, queue, expr, cron, opts) => recurringJobOptions = opts);

        // Act
        JobManager.AddOrUpdate(recurringJobId, jobExpression, cronExpression, options);

        // Assert
        MockRecurringJobManager.Verify(
            x => x.AddOrUpdate(
                recurringJobId,
                ExpectedQueue,
                jobExpression,
                cronExpression,
                It.IsAny<RecurringJobOptions>()),
            Times.Once);

        // Verify the options were correctly converted
        recurringJobOptions.ShouldNotBeNull();
    }


    //- - - - - - - - - - - - //


    [Fact]
    public void RemoveIfExists_Should_CallRecurringJobManager()
    {
        // Arrange
        var recurringJobId = "recurring-job-456";

        // Act
        JobManager.RemoveIfExists(recurringJobId);

        // Assert
        MockRecurringJobManager.Verify(
            x => x.RemoveIfExists(recurringJobId),
            Times.Once);
    }


    //- - - - - - - - - - - - //


    [Fact]
    public void GetRecurringJobs_Should_ReturnConvertedJobs()
    {
        // Arrange
        var methodInfo = typeof(TestJobHandler).GetMethod("DoWorkAsync");
        var recurringJobs = new List<RecurringJobDto>
        {
            new() {
                Id = "job1",
                Cron = "0 0 * * *",
                Queue = ExpectedQueue,
                Job = new Job(typeof(TestJobHandler), methodInfo)
            },
            new() {
                Id = "job2",
                Cron = "0 0 * * *",
                Queue = ExpectedQueue,
                Job = new Job(typeof(TestJobHandler), methodInfo)
            }
        };

        MockRecurringJobManager
            .Setup(x => x.GetRecurringJobs())
            .Returns(recurringJobs);

        // Act
        var result = JobManager.GetRecurringJobs();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(recurringJobs.Count);
        MockRecurringJobManager.Verify(x => x.GetRecurringJobs(), Times.Once);
    }


    //########################//


    // Helper test job handler class
    protected class TestJobHandler(string jobId = "test-job") : AMyIdJobHandler(jobId)
    {
        public Task DoWorkAsync()
        {
            return Task.CompletedTask;
        }
    }

}//Cls