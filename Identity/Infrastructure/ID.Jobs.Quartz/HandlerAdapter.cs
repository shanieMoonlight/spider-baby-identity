using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Collections.Concurrent;
using System.Reflection;

namespace ID.Jobs.Quartz;

[DisallowConcurrentExecution]
internal sealed class HandlerAdapter<THandler>(IServiceProvider _provider, ILogger<HandlerAdapter<THandler>> _logger) 
    : IJob where THandler : class
{

    // cache delegates per method name for this handler type
    private static readonly ConcurrentDictionary<string, Func<THandler, CancellationToken, Task>> _methodCache = new();

    public async Task Execute(IJobExecutionContext context)
    {
        var methodName = context.JobDetail.JobDataMap.GetString(QuartzConstants.MethodNameKey);
        if (string.IsNullOrWhiteSpace(methodName))
        {
            _logger.LogError("No method name provided in JobDataMap for handler {Handler}.", typeof(THandler).FullName);
            return;
        }


        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetService<THandler>();
        if (handler == null)
        {
            _logger.LogError("Handler type {Handler} not registered in DI.", typeof(THandler).FullName);
            return;
        }


        try
        {
            var func = _methodCache.GetOrAdd(methodName, CreateDelegateForMethod);
            if (func == null)
            {
                _logger.LogError("Unable to create delegate for method '{Method}' on handler {Handler}.", methodName, typeof(THandler).FullName);
                return;
            }

            await func(handler, context.CancellationToken).ConfigureAwait(false);
        }
        catch (TargetInvocationException tie) when (tie.InnerException != null)
        {
            _logger.LogError(tie.InnerException, "Unhandled exception in handler '{Handler}.{Method}'", typeof(THandler).FullName, methodName);
            throw tie.InnerException;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception invoking handler '{Handler}.{Method}'", typeof(THandler).FullName, methodName);
            throw;
        }
    }

    //-----------------------//

    private static Func<THandler, CancellationToken, Task> CreateDelegateForMethod(string methodName)
    {
        var method = typeof(THandler).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
            return null!;

        var parameters = method.GetParameters();

        // parameterless Task-returning method
        if (parameters.Length == 0 && method.ReturnType == typeof(Task))
            return (handler, ct) => (Task)method.Invoke(handler, [])!;

        // single CancellationToken parameter and Task return
        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(CancellationToken) && method.ReturnType == typeof(Task))
        {
            // Try to create a strongly-typed delegate for performance
            try
            {
                var dlg = (Func<THandler, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<THandler, CancellationToken, Task>), method);
                return dlg;
            }
            catch
            {
                // fallback to reflection wrapper
                return (handler, ct) => (Task)method.Invoke(handler, new object[] { ct })!;
            }
        }

        // void returning parameterless method
        if (parameters.Length != 0 || method.ReturnType != typeof(void))           
            return null!; // unsupported signature

        return (handler, ct) =>
        {
            method.Invoke(handler, []);
            return Task.CompletedTask;
        };
    }


}//Cls
