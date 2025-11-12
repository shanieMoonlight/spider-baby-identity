//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Quartz;
//using System.Reflection;

//namespace ID.Jobs.Quartz;

//[DisallowConcurrentExecution] // prevents overlapping executions for the same JobKey
//internal sealed class GenericQuartzJob(IServiceProvider provider, ILogger<GenericQuartzJob> logger) : IJob
//{
//    private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));
//    private readonly ILogger<GenericQuartzJob> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

//    //-----------------------//

//    public async Task Execute(IJobExecutionContext context)
//    {
//        var data = context.JobDetail.JobDataMap;
//        var handlerTypeName = data.GetString(QuartzConstants.HandlerTypeKey);
//        var methodName = data.GetString(QuartzConstants.MethodNameKey);

//        if (string.IsNullOrEmpty(handlerTypeName) || string.IsNullOrEmpty(methodName))
//        {
//            _logger.LogError("GenericQuartzJob missing HandlerType or MethodName in JobDataMap.");
//            return;
//        }

//        var handlerType = Type.GetType(handlerTypeName);
//        if (handlerType == null)
//        {
//            _logger.LogError("GenericQuartzJob cannot resolve handler type '{HandlerTypeName}'.", handlerTypeName);
//            return;
//        }

//        using var scope = _provider.CreateScope();
//        var handler = scope.ServiceProvider.GetService(handlerType);
//        if (handler == null)
//        {
//            _logger.LogError("GenericQuartzJob handler '{HandlerType}' not registered in DI.", handlerType.FullName);
//            return;
//        }


//        var method = handlerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//        if (method == null)
//        {
//            _logger.LogError("GenericQuartzJob method '{MethodName}' not found on type '{HandlerType}'.", methodName, handlerType.FullName);
//            return;
//        }



//        try
//        {
//            var result = method.Invoke(handler, []);
//            if (result is Task task)
//                await task.ConfigureAwait(false);
//            else
//                _logger.LogWarning("GenericQuartzJob invoked method '{MethodName}' on '{Handler}' which did not return Task.", methodName, handlerType.FullName);
//        }
//        catch (TargetInvocationException tie) when (tie.InnerException != null)
//        {
//            _logger.LogError(tie.InnerException, "Error while executing job handler '{Handler}.{Method}'", handlerType.FullName, methodName);
//            throw tie.InnerException;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error while executing job handler '{Handler}.{Method}'", handlerType.FullName, methodName);
//            throw;
//        }
//    }

//}
