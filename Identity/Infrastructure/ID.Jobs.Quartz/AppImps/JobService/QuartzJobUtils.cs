namespace ID.Jobs.Quartz.AppImps.JobService;
internal static  class QuartzJobUtils
{
    public static string GetHandlerTypeQualifiedName<THandler>()
    {
        var handlerType = typeof(THandler);
        return handlerType.AssemblyQualifiedName ?? throw new InvalidOperationException($"Cannot determine type name for handler: {handlerType}");
    }
}
