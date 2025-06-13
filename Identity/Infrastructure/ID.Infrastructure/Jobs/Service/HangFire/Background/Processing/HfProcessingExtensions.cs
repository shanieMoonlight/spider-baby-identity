using Hangfire.Storage.Monitoring;
using ID.Application.Jobs.Models;
using ID.Infrastructure.Jobs.Service.HangFire.Utility;

namespace ID.Infrastructure.Jobs.Service.HangFire.Background.Processing;


internal static class HfProcessingExtensions
{
    internal static List<IdProcessingJob> ToIdProcessingJobs(this IEnumerable<ProcessingJobDto> recJob) =>
        [.. recJob.Select(rj => rj.ToIdProcessingJob())];


    //- - - - - - - - - - - - - - - - - //


    internal static IdProcessingJob ToIdProcessingJob(this ProcessingJobDto recJob) => new()
    {
        Job = recJob.Job.ToIdJob(),
        ServerId = recJob.ServerId,
        StartedAt = recJob.StartedAt,
        LoadException = recJob.LoadException,
        StateData = recJob.StateData
    };



}//Cls
