using ID.Application.Jobs.Abstractions;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using System.Linq.Expressions;


namespace ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Imps;

/// <summary>
/// Abstract base class that wraps Hangfire's background job functionality.
/// Provides methods for creating, modifying, and managing background jobs
/// using a specific storage and queue.
/// </summary>
/// <param name="instanceProvider">The Job storage instance to use for job persistence</param>
/// <param name="queue">The name of the queue to use for jobs</param>
internal abstract class AHfBackgroundJobMgr(IHangfireInstanceProvider instanceProvider, string queue) : IHfBackgroundJobMgr
{
    private readonly IBackgroundJobClientWrapper _backgroundJobClient = instanceProvider.BackgroundJobClient;
    private readonly string _queue = queue;


    //- - - - - - - - - - - - //


    /// <summary>
    /// Creates a fire-and-forget background job and places it into the specified queue for immediate execution.
    /// </summary>
    /// <typeparam name="TJobHandler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <returns>A unique identifier of the created background job</returns>
    public string Enqueue<TJobHandler>(Expression<Func<TJobHandler, Task>> jobLambda)
        where TJobHandler : AMyIdJobHandler =>
        _backgroundJobClient.Enqueue(_queue, jobLambda);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Changes the state of a background job with the specified ID to the Enqueued state.
    /// This effectively makes a job available for immediate processing.
    /// </summary>
    /// <param name="jobId">The identifier of the job to requeue</param>
    /// <returns>True if the state change was successful, false otherwise</returns>
    public bool Requeue(string jobId) =>
        _backgroundJobClient.Requeue(jobId);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Creates a new background job and schedules it to be enqueued after the specified delay.
    /// </summary>
    /// <typeparam name="TJobHandler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <param name="delay">The time span to wait before enqueuing the job</param>
    /// <returns>A unique identifier of the created background job</returns>
    public string Schedule<TJobHandler>(Expression<Func<TJobHandler, Task>> jobLambda, TimeSpan delay)
        where TJobHandler : AMyIdJobHandler =>
        _backgroundJobClient.Schedule(_queue, jobLambda, delay);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Enqueues a delayed job.
    /// </summary>
    /// <typeparam name="TJobHandler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <param name="delay">The delay before the job should be executed.</param>
    /// <returns>The ID of the enqueued job.</returns>
    public bool Reschedule(string jobId, TimeSpan delay) =>
        _backgroundJobClient.Reschedule(jobId, delay);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Changes the state of a background job with the specified ID to the Deleted state.
    /// The job is not actually removed from storage but will not be executed.
    /// </summary>
    /// <param name="jobId">The identifier of the job to delete</param>
    /// <returns>True if the state change was successful, false otherwise</returns>
    public bool Delete(string jobId) =>
        _backgroundJobClient.Delete(jobId);



}//Cls
