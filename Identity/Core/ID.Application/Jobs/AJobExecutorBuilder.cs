using ID.Application.Jobs.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;

namespace ID.Application.Jobs;

//####################################################//


internal static class JobStarterBuilder
{
    public static JobStarter<Handler> BuildJobStarter<Handler>(this IServiceProvider provider)
        where Handler : AMyIdJobHandler
    {
        Handler handler = provider.GetRequiredService<Handler>();
        return new(provider, handler);
    }

}//Cls


//####################################################//


/// <summary>
/// Represents a job starter that facilitates the setup and management of recurring jobs.
/// </summary>
/// <typeparam name="Handler">The type of the job handler, which must inherit from <see cref="AMyIdJobHandler"/>.</typeparam>
internal class JobStarter<Handler>(IServiceProvider provider, Handler handler) where Handler : AMyIdJobHandler
{
    /// <summary>
    /// The job service used to manage recurring jobs.
    /// </summary>
    private readonly IMyIdJobService _jobService = provider.GetRequiredService<IMyIdJobService>();

    /// <summary>
    /// The hosting environment information.
    /// </summary>
    private readonly IWebHostEnvironment _env = provider.GetRequiredService<IWebHostEnvironment>();


    //---------------------------------//


    /// <summary>
    /// Creates a new instance of <see cref="JobStarter{Handler}"/>.
    /// </summary>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    /// <param name="handler">The job handler instance.</param>
    /// <returns>A new instance of <see cref="JobStarter{Handler}"/>.</returns>
    public static JobStarter<Handler> Create(ServiceProvider provider, Handler handler) =>
        new(provider, handler);


    //---------------------------------//


    /// <summary>
    /// Sets up a recurring job in a production environment.
    /// </summary>
    /// <param name="jobLambda">An expression representing the job to execute.</param>
    /// <param name="cronFrequencyExpression">The CRON expression defining the job's schedule.</param>
    /// <returns>The current instance of <see cref="JobStarter{Handler}"/>.</returns>
    public JobStarter<Handler> SetupRecurringProduction(Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression)
    {
        if (_env.IsDevelopment())
            return this;

        _jobService.StartRecurringJob(handler.JobId, jobLambda, cronFrequencyExpression);
        return this;
    }


    //---------------------------------//


    /// <summary>
    /// Sets up a recurring job in a development environment.
    /// </summary>
    /// <param name="jobLambda">An expression representing the job to execute.</param>
    /// <param name="cronFrequencyExpression">The CRON expression defining the job's schedule.</param>
    /// <returns>The current instance of <see cref="JobStarter{Handler}"/>.</returns>
    public JobStarter<Handler> SetupRecurringDevelopment(Expression<Func<Handler, Task>> jobLambda, string cronFrequencyExpression)
    {
        if (!_env.IsDevelopment())
            return this;

        _jobService.StartRecurringJob(handler.JobId, jobLambda, cronFrequencyExpression);
        return this;
    }
}

//####################################################//
