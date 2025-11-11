using Hangfire.Common;
using Hangfire.Server;
using ID.Application.Jobs.Abstractions;
using System.Globalization;
using System.Reflection;

namespace ID.Infrastructure.Jobs.Service.HangFire.Filters;

internal class MyIdDisableConcurrentExecutionFilter : JobFilterAttribute, IServerFilter
{
    private const string _distributedLockItemKey = "DistributedLock";

    public void OnPerforming(PerformingContext context)
    {
        var job = context.BackgroundJob?.Job;
        if (job == null) return;

        var attr = GetMyIdAttribute(job);
        if (attr == null) return;

        string resource = GetResource(job, attr);
        TimeSpan timeout = TimeSpan.FromSeconds(attr.TimeoutSec);

        IDisposable dlock = context.Connection.AcquireDistributedLock(resource, timeout);
        context.Items[_distributedLockItemKey] = dlock;
    }

    //---------------------//

    public void OnPerformed(PerformedContext context)
    {
        if (!context.Items.TryGetValue(_distributedLockItemKey, out var value))
            return;

        try
        {
            ((IDisposable)value).Dispose();
        }
        catch
        {
            // swallow
        }
    }

    //---------------------//

    private static MyIdDisableConcurrentExecutionAttribute? GetMyIdAttribute(Job job)
    {
        var type = job.Type;
        if (type == null) return null;

        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(m => m.Name == job.Method.Name);

        foreach (var method in methods)
        {
            var a = method.GetCustomAttribute<MyIdDisableConcurrentExecutionAttribute>(inherit: true);
            if (a != null) return a;
        }

        return type.GetCustomAttribute<MyIdDisableConcurrentExecutionAttribute>(inherit: true);
    }

    //---------------------//

    private static string GetResource(Job job, MyIdDisableConcurrentExecutionAttribute attr)
    {
        if (!string.IsNullOrWhiteSpace(attr.Resource))
        {
            try
            {
                return string.Format(CultureInfo.InvariantCulture, attr.Resource, [.. job.Args]).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                throw new FormatException("Unable to obtain resource identifier: " + ex.Message);
            }
        }

        var typeName = job.Type?.FullName ?? job.Type?.ToString() ?? "UnknownType";
        return (typeName + "." + job.Method.Name).ToLowerInvariant();
    }

}
//Cls