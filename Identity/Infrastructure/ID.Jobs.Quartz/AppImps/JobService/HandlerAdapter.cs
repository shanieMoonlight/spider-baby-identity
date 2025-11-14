using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Runtime.ExceptionServices;
using ID.Application.Jobs.Abstractions;

namespace ID.Jobs.Quartz.AppImps.JobService;

/// <summary>
/// Adapter that lets Quartz run strongly-typed handler classes as Quartz jobs.
///
/// Simplified: this adapter requires handlers to derive from <see cref="AMyIdJobHandler"/>
/// and invokes the uniform <c>HandleAsync</c> entry point.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class HandlerAdapter<THandler>(IServiceProvider _provider, ILogger<HandlerAdapter<THandler>> _logger)
    : IJob where THandler : AMyIdJobHandler
{

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetService<THandler>();
        if (handler == null)
        {
            _logger.LogError("Handler type {Handler} not registered in DI.", typeof(THandler).FullName);
            return;
        }

        try
        {
            // Invoke the canonical handler entrypoint. Handlers implement their own behavior.
            await handler.HandleAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception invoking handler '{Handler}.HandleAsync'", typeof(THandler).FullName);
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }

}
