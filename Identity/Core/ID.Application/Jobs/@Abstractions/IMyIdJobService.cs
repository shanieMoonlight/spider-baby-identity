using ID.Application.Jobs.Models;
using System.Linq.Expressions;

namespace ID.Application.Jobs.@Abstractions;

/// <summary>
/// Interface providing methods to manage background and recurring jobs in the Identity system.
/// Abstracts job operations such as creating, scheduling, retrieving and deleting jobs.
/// </summary>
public interface IMyIdJobService
{
    /// <summary>
    /// Retrieves all recurring jobs currently registered in the system.
    /// </summary>
    /// <returns>A list of recurring job information including schedules and status</returns>
    Task<List<IdRecurringJob>> GetRecurringJobsAsync();


    //- - - - - - - - - - - - //


    /// <summary>
    /// Creates or updates a recurring job that will execute based on the specified cron schedule.
    /// </summary>
    /// <typeparam name="Handler">The type of job handler that will process the job</typeparam>
    /// <param name="jobId">A unique identifier for the recurring job</param>
    /// <param name="jobLambda">The expression defining the method to be executed</param>
    /// <param name="cronFrequencyExpression">A cron expression that determines the job schedule</param>
    /// <returns>True if the operation was successful</returns>
    Task<bool> StartRecurringJob<Handler>(string jobId, Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression) where Handler : AMyIdJobHandler;


    //- - - - - - - - - - - - //


    /// <summary>
    /// Stops a recurring job with the specified ID by removing it from the schedule.
    /// </summary>
    /// <param name="jobId">The identifier of the recurring job to remove</param>
    /// <returns>True if the operation was successful</returns>
    Task<bool> StopRecurringJob(string jobId);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Creates a one-time job that will be executed after the specified delay.
    /// </summary>
    /// <typeparam name="Handler">The type of job handler that will process the job</typeparam>
    /// <param name="jobLambda">The expression defining the method to be executed</param>
    /// <param name="delay">The time to wait before executing the job</param>
    /// <returns>The ID of the created job</returns>
    Task<string> ScheduleJob<Handler>(Expression<Func<Handler, Task>> jobLambda, TimeSpan delay) where Handler : AMyIdJobHandler;


    //- - - - - - - - - - - - //


    /// <summary>
    /// Creates a one-time job for immediate execution in the background.
    /// </summary>
    /// <typeparam name="Handler">The type of job handler that will process the job</typeparam>
    /// <param name="jobLambda">The expression defining the method to be executed</param>
    /// <returns>The ID of the created job</returns>
    Task<string> EnqueueJob<Handler>(Expression<Func<Handler, Task>> jobLambda) where Handler : AMyIdJobHandler;


    //- - - - - - - - - - - - //


    /// <summary>
    /// Deletes a background job, preventing it from being executed if it hasn't started yet.
    /// </summary>
    /// <param name="jobId">The identifier of the job to delete</param>
    /// <returns>True if the delete operation was successful</returns>
    Task<bool> DeleteJob(string jobId);


}//Cls
