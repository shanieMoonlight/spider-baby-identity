using Hangfire;
using ID.Application.Jobs.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;


/// <summary>
/// Wrapper to make testing Extension methods easier for Hangfire background job client operations.
/// </summary>
internal interface IBackgroundJobClientWrapper
{

    /// <summary>
    ///Creates a background job based on a specified lambda expression and places it
    ///into the specified queue. Please, see the Hangfire.QueueAttribute to learn how
    ///to place the job on a non-default queue.
    /// </summary>
    /// <typeparam name="T">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="queue"> Queue for handling the background job.</param>
    /// <param name="methodCall">A lambda expression that defines the job method to execute</param>
    /// <returns>A unique identifier of the created background job</returns>
    string Enqueue<T>(string queue, Expression<Func<T, Task>> methodCall) where T : AMyIdJobHandler;

    //- - - - - - - - - - - - //

    /// <summary>
    /// Changes state of a job with the specified jobId to the Hangfire.States.EnqueuedState.
    /// </summary>
    /// <param name="jobId"> Identifier of job, whose state is being changed.</param>
    /// <returns> True, if state change succeeded, otherwise false.</returns>
    bool Requeue(string jobId);


    //- - - - - - - - - - - - //


    /// <summary>
    ///   Creates a new background job based on a specified instance method call expressi
    ///   and schedules it to be enqueued to the specified queue after a given delay.
    ///   </summary>
    /// <typeparam name="T">AMyIdJobHandler</typeparam>
    /// <param name="queue"> Queue for handling the background job.</param>
    /// <param name="methodCall"> Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay"> Delay, after which the job will be enqueued.</param>
    /// <returns>Unique identifier of the created job.</returns>
    string Schedule<T>([NotNull] string queue, Expression<Func<T, Task>> methodCall, TimeSpan delay) where T : AMyIdJobHandler;

    //- - - - - - - - - - - - //

    /// <summary>
    ///    Changes state of a job with the specified jobId to the Hangfire.States.ScheduledState.
    /// </summary>
    /// <typeparam name="T">AMyIdJobHandler</typeparam>
    /// <param name="jobId">Identifier of job, whose state is being changed.</param>
    /// <param name="delay"> Delay, after which the job will be enqueued.</param>
    /// <returns>Unique identifier of the created job.</returns>
    bool Reschedule(string jobId, TimeSpan delay);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Changes the state of a background job with the specified ID to the Deleted state.
    /// The job is not actually removed from storage but will not be executed.
    /// </summary>
    /// <param name="jobId">The identifier of the job to delete</param>
    /// <returns>True if the state change was successful, false otherwise</returns>
    bool Delete(string jobId);
}
