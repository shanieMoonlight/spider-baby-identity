using Hangfire.Common;
using Hangfire.Storage;
using ID.Application.Jobs.Models;
using ID.Infrastructure.Jobs.Service.HangFire.Utility;

namespace ID.Infrastructure.Jobs.Service.HangFire.Recurring.Utility;


internal static class HfRecurringExtensions
{
    internal static List<IdRecurringJob> ToIdRecurringJobs(this IEnumerable<RecurringJobDto> recJob) =>
        [.. recJob.Select(rj => rj.ToIdRecurringJob())];

    //- - - - - - - - - - - - - - - - - //

    internal static IdRecurringJob ToIdRecurringJob(this RecurringJobDto recJob) => new()
    {
        Id = recJob.Id,
        Job = recJob.Job.ToIdJob(),
        CreatedAt = recJob.CreatedAt,
        Cron = recJob.Cron,
        Error = recJob.Error,
        LastExecution = recJob.LastExecution,
        LastJobId = recJob.LastJobId,
        LastJobState = recJob.LastJobState,
        LoadException = recJob.LoadException,
        NextExecution = recJob.NextExecution,
        Queue = recJob.Queue,
        Removed = recJob.Removed,
        RetryAttempt = recJob.RetryAttempt,
        TimeZoneId = recJob.TimeZoneId
    };

}//Cls
