using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.Models;
using ID.Jobs.Quartz.Retries;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ID.Jobs.Quartz.AppImps.JobService;

internal sealed class QuartzMyIdJobService(
    ISchedulerFactory _schedulerFactory,
    ILogger<QuartzMyIdJobService> _logger,
    PendingRetryStore _store)
    : IMyIdJobService
{

    public async Task<bool> StartRecurringJob<Handler>(
        string jobId, Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression)
        where Handler : AMyIdJobHandler
    {
        try
        {
            await ScheduleRecurringJobCore(jobId, jobLambda, cronFrequencyExpression);

            _logger.LogInformation("Scheduled recurring job {JobId} with cron {Cron}", jobId, cronFrequencyExpression);
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"jobId:{jobId}");
            Debug.WriteLine($"cronFrequencyExpression:{cronFrequencyExpression}");
            Debug.WriteLine(e.Message);
            Debug.WriteLine(e.StackTrace);
            _logger.LogError(e, "Failed to schedule job {JobId}. (cron={CronFrequencyExpression}) (error={Error}.  trace={StackTrace})\r\nHave you migrated the database for quartz?",
                jobId, cronFrequencyExpression, e.Message, e.StackTrace);

            await StoreFailedJobAsync(jobId, jobLambda, cronFrequencyExpression);
            return false;
        }
    }

    //- - - - - - - - - - - -//

    /// <summary>
    /// This will be used in retries to prevent infinite loops.
    /// </summary>
    /// <exception cref="Exception">Quartz Exception if DB not ready</exception>
    private async Task ScheduleRecurringJobCore<Handler>(string jobId, Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression)
        where Handler : AMyIdJobHandler
    {
        var method = jobLambda.ExtractMethodInfo();
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);

        var jobData = new JobDataMap
        {
            [QuartzConstants.HandlerTypeKey] = QuartzJobUtils.GetHandlerTypeQualifiedName<Handler>(),
            [QuartzConstants.MethodNameKey] = method.Name
        };

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

        await scheduler.AddJob(jobDetail, replace: true);

        var existingTriggers = await scheduler.GetTriggersOfJob(jobKey);
        var firstTrigger = existingTriggers.FirstOrDefault();
        if (firstTrigger != null)
            await scheduler.RescheduleJob(firstTrigger.Key, trigger);
        else
            await scheduler.ScheduleJob(trigger);
    }

    //- - - - - - - - - - - -//

    private Task StoreFailedJobAsync<Handler>(string jobId, Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression)
          where Handler : AMyIdJobHandler
    {
        // Enqueue a retry action that will attempt the scheduling again when migrations succeed.
        var pending = new PendingRetry(
            ct => ScheduleRecurringJobCore(jobId, jobLambda, cronFrequencyExpression),
            Description: $"StartRecurringJob {jobId}",
            EnqueuedAt: DateTimeOffset.UtcNow);

        if (!_store.TryAdd(jobId, pending))
        {
            _logger.LogWarning("Pending retry {JobId} already present", jobId);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Stored pending retry {JobId} for later processing", jobId);

        return Task.CompletedTask;
    }


    //-----------------------//

    public async Task<bool> StopRecurringJob(string jobId)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);
        var result = await scheduler.DeleteJob(jobKey);
        _logger.LogInformation("Stopped recurring job {JobId} (deleted={Deleted})", jobId, result);
        return result;
    }

    //-----------------------//

    public async Task<List<IdRecurringJob>> GetRecurringJobsAsync()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var matcher = GroupMatcher<JobKey>.GroupEquals(QuartzConstants.JobGroup);
        var jobKeys = await scheduler.GetJobKeys(matcher);

        var result = new List<IdRecurringJob>();
        foreach (var jk in jobKeys)
        {
            var details = await scheduler.GetJobDetail(jk);
            var triggers = await scheduler.GetTriggersOfJob(jk);
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
        var method = jobLambda.ExtractMethodInfo();
        var scheduler = await _schedulerFactory.GetScheduler();

        var jobId = Guid.NewGuid().ToString("N");
        var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);

        var jobData = new JobDataMap
        {
            [QuartzConstants.HandlerTypeKey] = QuartzJobUtils.GetHandlerTypeQualifiedName<Handler>(),
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

        await scheduler.ScheduleJob(jobDetail, trigger);

        return jobId;
    }


    //-----------------------//


    public async Task<string> EnqueueJob<Handler>(Expression<Func<Handler, Task>> jobLambda) where Handler : AMyIdJobHandler =>
        await ScheduleJob(jobLambda, TimeSpan.Zero);


    //-----------------------//

    public async Task<bool> DeleteJob(string jobId)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobId, QuartzConstants.JobGroup);
        var result = await scheduler.DeleteJob(jobKey);
        _logger.LogInformation("Deleted job {JobId} (deleted={Deleted})", jobId, result);
        return result;
    }


}//Cls
