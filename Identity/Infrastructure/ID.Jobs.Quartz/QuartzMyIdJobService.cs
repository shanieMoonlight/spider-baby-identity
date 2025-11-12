using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ID.Jobs.Quartz;

internal sealed class QuartzMyIdJobService(ISchedulerFactory _schedulerFactory, ILogger<QuartzMyIdJobService> _logger)
    : IMyIdJobService
{

    private static MethodInfo ExtractMethodInfo<T>(Expression<Func<T, Task>> expression)
    {
        if (expression.Body is not MethodCallExpression mce)
            throw new NotSupportedException("Only method call expressions are supported (e.g. h => h.HandleAsync()).");

        if (mce.Arguments?.Count > 0)
            throw new NotSupportedException("Only parameterless handler methods are supported by this initial adapter.");

        return mce.Method;
    }

    //-----------------------//

    private async Task<IScheduler> GetScheduler() =>
        await _schedulerFactory.GetScheduler().ConfigureAwait(false);

    //-----------------------//

    public async Task<bool> StartRecurringJob<Handler>(
        string jobId, Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression)
        where Handler : AMyIdJobHandler
    {
        try
        {
            var method = ExtractMethodInfo(jobLambda);
            var scheduler = await GetScheduler().ConfigureAwait(false);

            var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);


            var jobData = new JobDataMap
            {
                [QuartzConstants.HandlerTypeKey] = GetHandlerTypeQualifiedName<Handler>(),
                [QuartzConstants.MethodNameKey] = method.Name
            };

            // Use typed handler adapter instead of global reflection job
            var adapterType = typeof(HandlerAdapter<>).MakeGenericType(typeof(Handler));

            var jobDetail = JobBuilder.Create(adapterType)
                .WithIdentity(jobKey)
                .UsingJobData(jobData)
                .StoreDurably()
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobId}.trigger", QuartzConstants.JobGroup)
                .ForJob(jobDetail)
                .WithCronSchedule(cronFrequencyExpression)
                .Build();

            await scheduler.AddJob(jobDetail, replace: true).ConfigureAwait(false);

            var existingTriggers = await scheduler.GetTriggersOfJob(jobKey).ConfigureAwait(false);
            if (existingTriggers != null && existingTriggers.Count != 0)
                await scheduler.RescheduleJob(existingTriggers.First().Key, trigger).ConfigureAwait(false);
            else
                await scheduler.ScheduleJob(trigger).ConfigureAwait(false);

            _logger.LogInformation("Scheduled recurring job {JobId} with cron {Cron}", jobId, cronFrequencyExpression);
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"jobId:{jobId}");
            Debug.WriteLine($"cronFrequencyExpression:{cronFrequencyExpression}");
            Debug.WriteLine(e.Message);
            Debug.WriteLine(e.StackTrace);
            throw;
        }
    }

    //-----------------------//

    public async Task<bool> StopRecurringJob(string jobId)
    {
        var scheduler = await GetScheduler().ConfigureAwait(false);
        var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);
        var result = await scheduler.DeleteJob(jobKey).ConfigureAwait(false);
        _logger.LogInformation("Stopped recurring job {JobId} (deleted={Deleted})", jobId, result);
        return result;
    }

    //-----------------------//

    public async Task<List<IdRecurringJob>> GetRecurringJobsAsync()
    {
        var scheduler = await GetScheduler().ConfigureAwait(false);
        var matcher = GroupMatcher<JobKey>.GroupEquals(QuartzConstants.JobGroup);
        var jobKeys = await scheduler.GetJobKeys(matcher).ConfigureAwait(false);

        var result = new List<IdRecurringJob>();
        foreach (var jk in jobKeys)
        {
            var details = await scheduler.GetJobDetail(jk).ConfigureAwait(false);
            var triggers = await scheduler.GetTriggersOfJob(jk).ConfigureAwait(false);
            var trigger = triggers.FirstOrDefault();

            var jobData = details?.JobDataMap ?? [];
            var cron = trigger is ICronTrigger cronTrigger ? cronTrigger.CronExpressionString ?? string.Empty : string.Empty;

            var handlerType = jobData.GetString(QuartzConstants.HandlerTypeKey) ?? string.Empty;
            var methodName = jobData.GetString(QuartzConstants.MethodNameKey) ?? string.Empty;

            var idJobDto = new IdJobDto(handlerType, methodName, []);

            result.Add(new IdRecurringJob
            {
                Id = jk.Name,
                Job = idJobDto,
                Cron = cron,
                Queue = null,
                CreatedAt = null
            });
        }

        return result;
    }

    //-----------------------//

    public async Task<string> ScheduleJob<Handler>(Expression<Func<Handler, Task>> jobLambda, TimeSpan delay) where Handler : AMyIdJobHandler
    {
        var method = ExtractMethodInfo(jobLambda);
        var scheduler = await GetScheduler().ConfigureAwait(false);

        var jobId = Guid.NewGuid().ToString("N");
        var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);

        var jobData = new JobDataMap
        {
            [QuartzConstants.HandlerTypeKey] = GetHandlerTypeQualifiedName<Handler>(),
            [QuartzConstants.MethodNameKey] = method.Name
        };

        var adapterType = typeof(HandlerAdapter<>).MakeGenericType(typeof(Handler));

        var jobDetail = JobBuilder.Create(adapterType)
            .WithIdentity(jobKey)
            .UsingJobData(jobData)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{jobId}.trigger", QuartzConstants.JobGroup)
            .ForJob(jobDetail)
            .StartAt(DateTimeOffset.UtcNow.Add(delay))
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger).ConfigureAwait(false);

        return jobId;
    }


    //-----------------------//


    public async Task<string> EnqueueJob<Handler>(Expression<Func<Handler, Task>> jobLambda) where Handler : AMyIdJobHandler =>
        await ScheduleJob(jobLambda, TimeSpan.Zero).ConfigureAwait(false);


    //-----------------------//

    public async Task<bool> DeleteJob(string jobId)
    {
        var scheduler = await GetScheduler().ConfigureAwait(false);
        var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);
        var result = await scheduler.DeleteJob(jobKey).ConfigureAwait(false);
        _logger.LogInformation("Deleted job {JobId} (deleted={Deleted})", jobId, result);
        return result;
    }


    //-----------------------//


    private static string GetHandlerTypeQualifiedName<THandler>()
    {
        var handlerType = typeof(THandler);
        return handlerType.AssemblyQualifiedName ?? throw new InvalidOperationException($"Cannot determine type name for handler: {handlerType}");
    }


}//Cls
