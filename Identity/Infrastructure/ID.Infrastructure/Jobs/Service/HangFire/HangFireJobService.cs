using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.Models;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using System.Linq.Expressions;

namespace ID.Infrastructure.Jobs.Service.HangFire;

internal class HangFireJobService(IHfDefaultRecurringJobMgr recurringMgr, IHfDefaultBackgroundJobMgr backgroundMgr) : IMyIdJobService
{

    public Task<bool> StartRecurringJob<Handler>(string jobId, Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression)
        where Handler : AMyIdJobHandler
    {
        recurringMgr.AddOrUpdate(
            jobId,
            jobLambda,
            cronFrequencyExpression);
        return Task.FromResult(true);
    }

    //- - - - - - - - - - - - - - - - - //

    public Task<bool> StopRecurringJob(string jobId)
    {
        recurringMgr.RemoveIfExists(jobId);
        return Task.FromResult(true);
    }

    //- - - - - - - - - - - - - - - - - //

    public Task<List<IdRecurringJob>> GetRecurringJobsAsync() =>
        Task.FromResult(recurringMgr.GetRecurringJobs());

    //- - - - - - - - - - - - - - - - - //

    public Task<string> ScheduleJob<Handler>(Expression<Func<Handler, Task>> jobLambda, TimeSpan delay) where Handler : AMyIdJobHandler
    {
        var jobId = backgroundMgr.Schedule(jobLambda, delay);
        return Task.FromResult(jobId);
    }

    //- - - - - - - - - - - - - - - - - //

    public Task<bool> RescheduleJob(string jobId, TimeSpan delay)
    {
        var requeueResult = backgroundMgr.Reschedule(jobId, delay);
        return Task.FromResult(requeueResult);
    }

    //- - - - - - - - - - - - - - - - - //

    public Task<string> EnqueueJob<Handler>(Expression<Func<Handler, Task>> jobLambda) where Handler : AMyIdJobHandler
    {
        var jobId = backgroundMgr.Enqueue(jobLambda);
        return Task.FromResult(jobId);
    }

    //- - - - - - - - - - - - - - - - - //

    public Task<bool> RequeueJob(string jobId)
    {
        var requeueResult = backgroundMgr.Requeue(jobId);
        return Task.FromResult(requeueResult);
    }

    //- - - - - - - - - - - - - - - - - //

    public Task<bool> DeleteJob(string jobId)
    {
        var deleteResult = backgroundMgr.Delete(jobId);
        return Task.FromResult(deleteResult);
    }

}//Cls