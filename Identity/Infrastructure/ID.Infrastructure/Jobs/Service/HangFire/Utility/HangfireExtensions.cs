using Hangfire.Common;
using ID.Application.Jobs.Models;

namespace ID.Infrastructure.Jobs.Service.HangFire.Utility;

internal static class HangfireExtensions
{

    internal static List<IdJobDto> ToIdJobs(this IEnumerable<Job> recJob) =>
        [.. recJob.Select(rj => rj.ToIdJob())];

    //- - - - - - - - - - - - - - - - - //

    internal static IdJobDto ToIdJob(this Job job) =>
        new(job.Type.Name, job.Method.Name, job.Args);


}//Cls