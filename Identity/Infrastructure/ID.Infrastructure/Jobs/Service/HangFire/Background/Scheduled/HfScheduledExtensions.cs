using Hangfire.Storage.Monitoring;
using ID.Application.Jobs.Models;
using ID.Infrastructure.Jobs.Service.HangFire.Utility;

namespace ID.Infrastructure.Jobs.Service.HangFire.Background.Scheduled;


internal static class HfScheduledExtensions
{
    internal static List<IdScheduledJob> ToIdScheduledJobs(this IEnumerable<ScheduledJobDto> recJob) =>
        [.. recJob.Select(rj => rj.ToIdScheduledJob())];


    //- - - - - - - - - - - - - - - - - //


    internal static IdScheduledJob ToIdScheduledJob(this ScheduledJobDto recJob) => new()
    {
        Job = recJob.Job.ToIdJob(),
        EnqueueAt = recJob.EnqueueAt,
        ScheduledAt = recJob.ScheduledAt,
        LoadException = recJob.LoadException,
        InScheduledState = recJob.InScheduledState,
        StateData = recJob.StateData
    };



}//Cls
