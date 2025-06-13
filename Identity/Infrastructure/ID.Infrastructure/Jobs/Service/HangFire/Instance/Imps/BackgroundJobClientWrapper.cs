using Hangfire;
using ID.Application.Jobs.Abstractions;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ID.Infrastructure.Jobs.Service.HangFire.Instance.Imps;
internal class BackgroundJobClientWrapper(JobStorage storage) : IBackgroundJobClientWrapper
{

    private readonly IBackgroundJobClient _backgroundJobClient = new BackgroundJobClient(storage);


    //- - - - - - - - - - - - //


    public bool Delete(string jobId) =>
        _backgroundJobClient.Delete(jobId);


    public string Enqueue<T>(string queue, Expression<Func<T, Task>> jobLambda) where T : AMyIdJobHandler =>
        _backgroundJobClient.Enqueue(queue, jobLambda);


    public bool Requeue(string jobId) =>
        _backgroundJobClient.Requeue(jobId);


    public bool Reschedule(string jobId, TimeSpan delay) =>
        _backgroundJobClient.Reschedule(jobId, delay);


    public string Schedule<T>([NotNull] string queue, Expression<Func<T, Task>> jobLambda, TimeSpan delay) where T : AMyIdJobHandler =>
        _backgroundJobClient.Schedule(queue, jobLambda, delay);


}//Cls
