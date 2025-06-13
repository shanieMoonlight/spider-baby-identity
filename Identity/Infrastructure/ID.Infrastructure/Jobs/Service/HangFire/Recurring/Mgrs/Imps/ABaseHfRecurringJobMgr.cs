using Hangfire;
using Hangfire.Storage;
using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.Models;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Models;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Utility;
using System.Linq.Expressions;


namespace ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Imps;

/// <summary>
/// Abstract base class for Hangfire recurring job management operations.
/// Provides core functionality for scheduling, removing and retrieving recurring jobs.
/// </summary>
/// <param name="instanceProvider">The Job storage instance to use for job persistence</param>
/// <param name="queue">The queue name where recurring jobs will be enqueued</param>
internal abstract class AHfRecurringJobMgr(IHangfireInstanceProvider instanceProvider, string queue) : IHfRecurringJobMgr
{
    private readonly IRecurringJobManagerWrapper _recurringJobManager = instanceProvider.RecurringJobManager;
    private readonly string _queue = queue;


    //- - - - - - - - - - - - //


    /// <summary>
    /// Adds or updates a recurring job with the specified job handler.
    /// </summary>
    /// <typeparam name="Handler">The type of the job handler, must inherit from AMyIdJobHandler</typeparam>
    /// <param name="recurringJobId">A unique identifier for the recurring job</param>
    /// <param name="jobLambda">A lambda expression that defines the job method to execute</param>
    /// <param name="cronExpression">A CRON expression that specifies when the job should run</param>
    /// <param name="options">Optional settings for the recurring job execution</param>
    public void AddOrUpdate<Handler>(
        string recurringJobId,
        Expression<Func<Handler, Task>> jobLambda,
        string cronExpression,
        HfRecurringJobOptions? options = null)
        where Handler : AMyIdJobHandler
    {
        _recurringJobManager.AddOrUpdate(
            recurringJobId,
            _queue,
            jobLambda,
            cronExpression,
            options.ToRecurringJobOptions());
    }


    //- - - - - - - - - - - - //


    /// <summary>
    /// Removes the recurring job with the specified ID if it exists.
    /// </summary>
    /// <param name="recurringJobId">The ID of the recurring job to remove</param>
    public void RemoveIfExists(string recurringJobId) =>
        _recurringJobManager.RemoveIfExists(recurringJobId);


    //- - - - - - - - - - - - //


    /// <summary>
    /// Retrieves all recurring jobs from the storage.
    /// </summary>
    /// <returns>A list of recurring job DTOs representing the current state of recurring jobs</returns>
    public List<IdRecurringJob> GetRecurringJobs() =>
        _recurringJobManager
            .GetRecurringJobs()
            .ToIdRecurringJobs();
}//Cls
