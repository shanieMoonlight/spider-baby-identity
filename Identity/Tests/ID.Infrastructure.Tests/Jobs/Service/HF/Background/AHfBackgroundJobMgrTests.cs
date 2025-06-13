using ID.Application.Jobs.Abstractions;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using Moq;
using System.Linq.Expressions;

namespace ID.Infrastructure.Tests.Jobs.Service.HF.Background;

/// <summary>
/// Base test class for background job manager implementations
/// </summary>
/// <typeparam name="TManager">The concrete manager type to test</typeparam>
public abstract class AHfBackgroundJobMgrTests   
{
    internal readonly Mock<IHangfireInstanceProvider> MockInstanceProvider;
    internal readonly Mock<IBackgroundJobClientWrapper> MockBackgroundJobClient;
    internal readonly IHfBackgroundJobMgr JobManager;
    protected readonly string ExpectedQueue;

    protected AHfBackgroundJobMgrTests(string expectedQueue)
    {
        ExpectedQueue = expectedQueue;
        
        // Setup mocks
        MockBackgroundJobClient = new Mock<IBackgroundJobClientWrapper>();
        
        MockInstanceProvider = new Mock<IHangfireInstanceProvider>();
        MockInstanceProvider
            .Setup(x => x.BackgroundJobClient)
            .Returns(MockBackgroundJobClient.Object);

        // Create the concrete manager through the factory method
        JobManager = CreateJobManager(MockInstanceProvider.Object);
    }



    //########################//

    /// <summary>
    /// Factory method to create the concrete manager implementation
    /// </summary>
    internal abstract IHfBackgroundJobMgr CreateJobManager(IHangfireInstanceProvider instanceProvider);


    //- - - - - - - - - - - - //


    [Fact]
    public abstract void JobManager_Should_ImplementCorrectInterface();



    //########################//


    [Fact]
    public void Enqueue_Should_UseCorrectQueue()
    {
        // Arrange
        Expression<Func<TestJobHandler, Task>> jobExpression = j => j.DoWorkAsync();
        var expectedJobId = "job123";

        MockBackgroundJobClient
            .Setup(x => x.Enqueue(ExpectedQueue, jobExpression))
            .Returns(expectedJobId);

        // Act
        var jobId = JobManager.Enqueue(jobExpression);

        // Assert
        jobId.ShouldBe(expectedJobId);
        MockBackgroundJobClient.Verify(
            x => x.Enqueue(ExpectedQueue, jobExpression), 
            Times.Once);
    }

    //- - - - - - - - - - - - //

    [Fact]
    public void Schedule_Should_UseCorrectQueue()
    {
        // Arrange
        Expression<Func<TestJobHandler, Task>> jobExpression = j => j.DoWorkAsync();
        var delay = TimeSpan.FromMinutes(5);
        var expectedJobId = "job456";

        MockBackgroundJobClient
            .Setup(x => x.Schedule(ExpectedQueue, jobExpression, delay))
            .Returns(expectedJobId);

        // Act
        var jobId = JobManager.Schedule(jobExpression, delay);

        // Assert
        jobId.ShouldBe(expectedJobId);
        MockBackgroundJobClient.Verify(
            x => x.Schedule(ExpectedQueue, jobExpression, delay),
            Times.Once);
    }

    //- - - - - - - - - - - - //

    [Fact]
    public void Requeue_Should_CallBackgroundJobClient()
    {
        // Arrange
        var jobId = "job789";
        MockBackgroundJobClient
            .Setup(x => x.Requeue(jobId))
            .Returns(true);

        // Act
        var result = JobManager.Requeue(jobId);

        // Assert
        result.ShouldBeTrue();
        MockBackgroundJobClient.Verify(x => x.Requeue(jobId), Times.Once);
    }

    //- - - - - - - - - - - - //

    [Fact]
    public void Reschedule_Should_CallBackgroundJobClient()
    {
        // Arrange
        var jobId = "job101112";
        var delay = TimeSpan.FromHours(1);
        MockBackgroundJobClient
            .Setup(x => x.Reschedule(jobId, delay))
            .Returns(true);

        // Act
        var result = JobManager.Reschedule(jobId, delay);

        // Assert
        result.ShouldBeTrue();
        MockBackgroundJobClient.Verify(x => x.Reschedule(jobId, delay), Times.Once);
    }

    //- - - - - - - - - - - - //

    [Fact]
    public void Delete_Should_CallBackgroundJobClient()
    {
        // Arrange
        var jobId = "job131415";
        MockBackgroundJobClient
            .Setup(x => x.Delete(jobId))
            .Returns(true);

        // Act
        var result = JobManager.Delete(jobId);

        // Assert
        result.ShouldBeTrue();
        MockBackgroundJobClient.Verify(x => x.Delete(jobId), Times.Once);
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