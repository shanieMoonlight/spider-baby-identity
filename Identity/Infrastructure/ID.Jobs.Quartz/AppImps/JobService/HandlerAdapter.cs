using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace ID.Jobs.Quartz.AppImps.JobService;

/// <summary>
/// Adapter that lets Quartz run strongly-typed handler classes as Quartz jobs.
///
/// Why this exists:
/// - Application code registers handlers (types derived from your handler base) and Quartz stores
///   the handler type + method name in the JobDataMap. Quartz needs an <see cref="IJob"/>
///   implementation to invoke the handler at runtime. This adapter resolves the handler from DI,
///   finds the requested method and invokes it.
///
/// Responsibilities:
/// - Resolve a handler instance from the DI scope for each job execution.
/// - Convert supported handler method signatures into a fast delegate and cache it per-method
///   so reflection is only used on first invocation.
/// - Translate handler exceptions into logged errors and rethrow so Quartz records failures.
///
/// Supported handler method signatures (on the handler type):
/// - `Task Method()`
/// - `Task Method(CancellationToken ct)`
/// - `void Method()` (synchronously executed and wrapped as completed Task)
///
/// The adapter is marked with <see cref="DisallowConcurrentExecutionAttribute"/>, which tells
/// Quartz not to run multiple instances of the same job concurrently.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class HandlerAdapter<THandler>(IServiceProvider _provider, ILogger<HandlerAdapter<THandler>> _logger)
    : IJob where THandler : class
{
    // Cache of generated delegates for (handler type, method name). Using a ConcurrentDictionary
    // avoids repeated reflection and Delegate.CreateDelegate overhead on every execution.
    private static readonly ConcurrentDictionary<string, Func<THandler, CancellationToken, Task>> _methodCache = new();


    //-----------------------//

    /// <summary>
    /// Entry point called by Quartz when the job fires.
    /// </summary>
    public async Task Execute(IJobExecutionContext context)
    {
        // Read the method name from JobDataMap (written when jobs are scheduled).
        var methodName = context.JobDetail.JobDataMap.GetString(QuartzConstants.MethodNameKey);
        if (string.IsNullOrWhiteSpace(methodName))
        {
            _logger.LogError("No method name provided in JobDataMap for handler {Handler}.", typeof(THandler).FullName);
            return;
        }

        // Create a DI scope and resolve the handler instance for this execution.
        using var scope = _provider.CreateScope();
        var handler = scope.ServiceProvider.GetService<THandler>();
        if (handler == null)
        {
            _logger.LogError("Handler type {Handler} not registered in DI.", typeof(THandler).FullName);
            return;
        }

        try
        {
            // Try get a cached delegate for the method name. If missing, create and cache one.
            if (!_methodCache.TryGetValue(methodName, out var func))
            {
                var created = CreateDelegateForMethod(methodName);
                if (created == null)
                {
                    var ex = new InvalidOperationException($"Unsupported handler signature for {typeof(THandler).FullName}.{methodName}");
                    _logger.LogError(ex, "Unsupported handler signature for {Handler}.{Method}", typeof(THandler).FullName, methodName);

                    // Cache a sentinel delegate that throws so subsequent calls fail fast in the same way.
                    Task throwing(THandler h, CancellationToken ct) => throw ex;
                    _methodCache.TryAdd(methodName, throwing);

                    // Re-throw now so Quartz records a failure for this execution.
                    throw ex;
                }

                _methodCache.TryAdd(methodName, created);
                func = created;
            }

            // Invoke the delegate with the handler instance and the job's cancellation token.
            await func(handler, context.CancellationToken);
        }
        catch (TargetInvocationException tie) when (tie.InnerException != null)
        {
            // If the handler delegate invoked a method that threw, unwrap and rethrow the inner
            // exception while preserving the original stack trace so Quartz can observe real cause.
            _logger.LogError(tie.InnerException, "Unhandled exception in handler '{Handler}.{Method}'", typeof(THandler).FullName, methodName);
            ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
        }
        catch (Exception ex)
        {
            // Any other exception should be logged and bubbled up so Quartz can mark the job as failed.
            _logger.LogError(ex, "Unhandled exception invoking handler '{Handler}.{Method}'", typeof(THandler).FullName, methodName);
            throw;
        }
    }

    //-----------------------//

    /// <summary>
    /// Create a uniform delegate for supported handler method signatures.
    /// Returns <c>null</c> if the method doesn't exist or its signature is unsupported.
    ///
    /// Supported forms:
    /// - Task Method()
    /// - Task Method(CancellationToken)
    /// - void Method()
    ///
    /// The implementation prefers a strongly-typed delegate (fast path) and falls back to a
    /// reflection wrapper when CreateDelegate fails.
    /// </summary>
    private static Func<THandler, CancellationToken, Task>? CreateDelegateForMethod(string methodName)
    {
        var method = typeof(THandler).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
            return null;

        var parameters = method.GetParameters();

        // 1) parameterless Task-returning method: Task Method()
        if (parameters.Length == 0 && method.ReturnType == typeof(Task))
            return (handler, ct) => (Task)method.Invoke(handler, [])!;

        // 2) single CancellationToken parameter and Task return: Task Method(CancellationToken)
        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(CancellationToken) && method.ReturnType == typeof(Task))
        {
            // Fast path: create a strongly-typed delegate for better performance.
            try
            {
                var dlg = (Func<THandler, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<THandler, CancellationToken, Task>), method);
                return dlg;
            }
            catch
            {
                // Fallback: reflection wrapper that invokes the method with the cancellation token.
                return (handler, ct) => (Task)method.Invoke(handler, [ct])!;
            }
        }

        // 3) void returning parameterless method: void Method()
        //    We treat this as synchronous work and return Task.CompletedTask after invoking.
        if (parameters.Length != 0 || method.ReturnType != typeof(void))
            return null; // unsupported signature

        return (handler, ct) =>
        {
            method.Invoke(handler, []);
            return Task.CompletedTask;
        };
    }

}
