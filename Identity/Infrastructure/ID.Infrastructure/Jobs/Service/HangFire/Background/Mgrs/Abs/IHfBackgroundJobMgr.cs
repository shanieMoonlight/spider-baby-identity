using ID.Application.Jobs.Abstractions;
using System.Linq.Expressions;

namespace ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;


/// <summary>
/// Interface for handling background jobs.
/// Provides methods for creating, modifying, and managing background jobs
/// </summary>
internal interface IHfBackgroundJobMgr
{
    /// <summary>
    /// Changes the state of a background job with the specified ID to the Deleted state.
    /// The job is not actually removed from storage but will not be executed.
    /// </summary>
    /// <param name="jobId">The identifier of the job to delete</param>
    /// <returns>True if the state change was successful, false otherwise</returns>
    bool Delete(string jobId);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Creates a fire-and-forget background job and places it into the specified queue for immediate execution.
    /// </summary>
    /// <typeparam name="TJobHandler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <returns>A unique identifier of the created background job</returns>
    string Enqueue<TJobHandler>(Expression<Func<TJobHandler, Task>> jobLambda) where TJobHandler : AMyIdJobHandler;

    //- - - - - - - - - - - - //

    /// <summary>
    /// Changes the state of a background job with the specified ID to the Enqueued state.
    /// This effectively makes a job available for immediate processing.
    /// </summary>
    /// <param name="jobId">The identifier of the job to requeue</param>
    /// <returns>True if the state change was successful, false otherwise</returns>
    bool Requeue(string jobId);


    //- - - - - - - - - - - - //

    /// <summary>
    /// Enqueues a delayed job.
    /// </summary>
    /// <typeparam name="TJobHandler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <param name="delay">The delay before the job should be executed.</param>
    /// <returns>The ID of the enqueued job.</returns>
    bool Reschedule(string jobId, TimeSpan delay);

    /// <summary>
    /// Creates a new background job and schedules it to be enqueued after the specified delay.
    /// </summary>
    /// <typeparam name="TJobHandler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <param name="delay">The time span to wait before enqueuing the job</param>
    /// <returns>A unique identifier of the created background job</returns>
    string Schedule<TJobHandler>(Expression<Func<TJobHandler, Task>> jobLambda, TimeSpan delay) where TJobHandler : AMyIdJobHandler;

}//Cls