using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.Models;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Models;
using System.Linq.Expressions;

namespace ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
/// <summary>
/// Interface for Hangfire recurring job management operations.
/// Provides core functionality for scheduling, removing and retrieving recurring jobs.
/// </summary>
internal interface IHfRecurringJobMgr
{
    void AddOrUpdate<Handler>(string recurringJobId, Expression<Func<Handler, Task>> jobLambda, string cronExpression, HfRecurringJobOptions? options = null) where Handler : AMyIdJobHandler;
    List<IdRecurringJob> GetRecurringJobs();
    void RemoveIfExists(string recurringJobId);
}