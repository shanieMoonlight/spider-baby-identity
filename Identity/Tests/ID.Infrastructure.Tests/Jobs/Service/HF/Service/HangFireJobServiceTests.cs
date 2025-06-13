using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.Models;
using ID.Infrastructure.Jobs.Service.HangFire;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using Moq;
using System.Linq.Expressions;

namespace ID.Infrastructure.Tests.Jobs.Service.HF.Service;

public class HangFireJobServiceTests
{
    private readonly Mock<IHfDefaultRecurringJobMgr> _mockRecurringMgr;
    private readonly Mock<IHfDefaultBackgroundJobMgr> _mockBackgroundMgr;
    private readonly HangFireJobService _service;

    public HangFireJobServiceTests()
    {
        _mockRecurringMgr = new Mock<IHfDefaultRecurringJobMgr>();
        _mockBackgroundMgr = new Mock<IHfDefaultBackgroundJobMgr>();
        _service = new HangFireJobService(
            _mockRecurringMgr.Object,
            _mockBackgroundMgr.Object);
    }

    #region Recurring Jobs

    [Fact]
    public async Task StartRecurringJob_ShouldCallAddOrUpdate()
    {
        // Arrange
        string jobId = "test-job-id";
        string cronExpression = "* * * * *";
        Expression<Func<TestJobHandler, Task>> jobLambda = h => h.HandleAsync();

        // Act
        var result = await _service.StartRecurringJob(jobId, jobLambda, cronExpression);

        // Assert
        _mockRecurringMgr.Verify(m => m.AddOrUpdate(
            jobId,
            jobLambda,
            cronExpression,
            null), Times.Once);
        result.ShouldBeTrue();
    }



    [Fact]
    public async Task StopRecurringJob_ShouldCallRemoveIfExists()
    {
        // Arrange
        string jobId = "test-job-id";

        // Act
        var result = await _service.StopRecurringJob(jobId);

        // Assert
        _mockRecurringMgr.Verify(m => m.RemoveIfExists(jobId), Times.Once);
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task GetRecurringJobsAsync_ShouldCallGetRecurringJobs()
    {
        // Arrange
        var expectedJobs = new List<IdRecurringJob> { CreateTestRecurringJob("test-job-id") };
        _mockRecurringMgr.Setup(m => m.GetRecurringJobs()).Returns(expectedJobs);

        // Act
        var result = await _service.GetRecurringJobsAsync();

        // Assert
        _mockRecurringMgr.Verify(m => m.GetRecurringJobs(), Times.Once);
        result.ShouldBeSameAs(expectedJobs);
    }

    #endregion

    #region Background Jobs

    [Fact]
    public async Task ScheduleJob_ShouldCallSchedule()
    {
        // Arrange
        Expression<Func<TestJobHandler, Task>> jobLambda = h => h.HandleAsync();
        var delay = TimeSpan.FromMinutes(5);
        var expectedJobId = "scheduled-job-id";
        _mockBackgroundMgr.Setup(m => m.Schedule(jobLambda, delay)).Returns(expectedJobId);

        // Act
        var result = await _service.ScheduleJob(jobLambda, delay);

        // Assert
        _mockBackgroundMgr.Verify(m => m.Schedule(jobLambda, delay), Times.Once);
        result.ShouldBe(expectedJobId);
    }

    [Fact]
    public async Task RescheduleJob_ShouldCallReschedule()
    {
        // Arrange
        string jobId = "test-job-id";
        var delay = TimeSpan.FromMinutes(10);
        _mockBackgroundMgr.Setup(m => m.Reschedule(jobId, delay)).Returns(true);

        // Act
        var result = await _service.RescheduleJob(jobId, delay);

        // Assert
        _mockBackgroundMgr.Verify(m => m.Reschedule(jobId, delay), Times.Once);
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task EnqueueJob_ShouldCallEnqueue()
    {
        // Arrange
        Expression<Func<TestJobHandler, Task>> jobLambda = h => h.HandleAsync();
        var expectedJobId = "enqueued-job-id";
        _mockBackgroundMgr.Setup(m => m.Enqueue(jobLambda)).Returns(expectedJobId);

        // Act
        var result = await _service.EnqueueJob(jobLambda);

        // Assert
        _mockBackgroundMgr.Verify(m => m.Enqueue(jobLambda), Times.Once);
        result.ShouldBe(expectedJobId);
    }

    [Fact]
    public async Task RequeueJob_ShouldCallRequeue()
    {
        // Arrange
        string jobId = "test-job-id";
        _mockBackgroundMgr.Setup(m => m.Requeue(jobId)).Returns(true);

        // Act
        var result = await _service.RequeueJob(jobId);

        // Assert
        _mockBackgroundMgr.Verify(m => m.Requeue(jobId), Times.Once);
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteJob_ShouldCallDelete()
    {
        // Arrange
        string jobId = "test-job-id";
        _mockBackgroundMgr.Setup(m => m.Delete(jobId)).Returns(true);

        // Act
        var result = await _service.DeleteJob(jobId);

        // Assert
        _mockBackgroundMgr.Verify(m => m.Delete(jobId), Times.Once);
        result.ShouldBeTrue();
    }

    #endregion

    #region Test Helpers

    // Helper method to create a test IdRecurringJob with all required fields
    private static IdRecurringJob CreateTestRecurringJob(string id, string type = "TestJobHandler", string method = "HandleAsync")
    {
        return new IdRecurringJob
        {
            Id = id,
            Cron = "* * * * *", // Every minute
            Job = new IdJobDto(type, method, []),
            Queue = "default",
            CreatedAt = DateTime.UtcNow,
            NextExecution = DateTime.UtcNow.AddMinutes(1)
        };
    }

    private class TestJobHandler(string jobId) : AMyIdJobHandler(jobId)
    {
        public Task HandleAsync()
        {
            return Task.CompletedTask;
        }
    }

    #endregion
}

//// Simple implementation of IdJobDto if not available in the test project
//public class IdJobDto
//{
//    public string Type { get; set; } = string.Empty;
//    public string Method { get; set; } = string.Empty;
//    public IEnumerable<object> Args { get; set; } = Array.Empty<object>();
//}


// Test job handler implementation
