using Quartz;
using System.Linq.Expressions;
using System.Reflection;


namespace ID.Jobs.Quartz.AppImps.JobService;
internal static  class QuartzJobUtils
{
    public static string GetHandlerTypeQualifiedName<THandler>()
    {
        var handlerType = typeof(THandler);
        return handlerType.AssemblyQualifiedName ?? throw new InvalidOperationException($"Cannot determine type name for handler: {handlerType}");
    }

    //- - - - - - - - - - - -//

    public static MethodInfo ExtractMethodInfo<T>(this Expression<Func<T, Task>> expression)
    {
        if (expression.Body is not MethodCallExpression mce)
            throw new NotSupportedException("Only method call expressions are supported (e.g. h => h.HandleAsync()).");

        if (mce.Arguments?.Count > 0)
            throw new NotSupportedException("Only parameterless handler methods are supported by this initial adapter.");

        return mce.Method;
    }

    //- - - - - - - - - - - -//

    public static async Task<IScheduler> GetScheduler(this ISchedulerFactory _schedulerFactory) =>
        await _schedulerFactory.GetScheduler();

}
