using Hangfire;
using Hangfire.Storage;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ID.Infrastructure.Jobs.Service.HangFire.Instance.Imps;
internal class RecurringJobManagerWrapper(JobStorage storage) : IRecurringJobManagerWrapper
{
    private readonly RecurringJobManager _recurringJobManager = new(storage);
    private readonly JobStorage _storage = storage;


    //- - - - - - - - - - - - //


    public void AddOrUpdate<T>(string recurringJobId, string queue, Expression<Func<T, Task>> methodCall, string cronExpression, RecurringJobOptions options) =>
        _recurringJobManager.AddOrUpdate(
            recurringJobId,
            queue,
            methodCall,
            cronExpression,
            options);


    public void RemoveIfExists(string recurringJobId) =>
        _recurringJobManager.RemoveIfExists(recurringJobId);


    public void Trigger(string recurringJobId) =>
        _recurringJobManager.Trigger(recurringJobId);



    public List<RecurringJobDto> GetRecurringJobs() =>
      _storage.GetConnection()
          .GetRecurringJobs();



}//Cls
