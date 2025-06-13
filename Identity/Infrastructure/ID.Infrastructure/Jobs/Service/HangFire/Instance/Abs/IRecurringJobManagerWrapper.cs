using Hangfire;
using Hangfire.Storage;
using System.Linq.Expressions;

namespace ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;

/// <summary>
/// Wrapper to make testing Extension methods easier for Hangfire Recurring job manager operations.
/// </summary>
internal interface IRecurringJobManagerWrapper
{

    /// <summary>
    /// Adds or updates a recurring job with the specified job handler.
    /// </summary>
    /// <typeparam name="Handler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="queue"> Queue for handling the background job.</param>
    /// <param name="recurringJobId">A unique identifier for the recurring job</param>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <param name="cronExpression">A CRON expression that specifies when the job should run</param>
    /// <param name="options">Optional settings for the recurring job execution</param>
    void AddOrUpdate<T>(
            string recurringJobId, 
            string queue, 
            Expression<Func<T, Task>> jobLambda, 
            string cronExpression, 
            RecurringJobOptions options);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Trigger existing recurring job Right Now
    /// </summary>
    /// <param name="recurringJobId"></param>
    void Trigger(string recurringJobId);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Removes the recurring job with the specified ID if it exists.
    /// </summary>
    /// <param name="recurringJobId">The ID of the recurring job to remove</param>
    void RemoveIfExists(string recurringJobId);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Retrieves all recurring jobs from the storage.
    /// </summary>
    /// <returns>A list of recurring job DTOs representing the current state of recurring jobs</returns>
    List<RecurringJobDto> GetRecurringJobs();


}//Cls
