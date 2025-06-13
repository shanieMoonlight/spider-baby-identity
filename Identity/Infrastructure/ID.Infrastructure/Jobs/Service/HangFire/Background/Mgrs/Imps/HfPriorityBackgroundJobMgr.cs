using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Utility;


namespace ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Imps;

/// <summary>
/// Implementation of the background job manager that uses the Priority Queue for Hangfire integration.
/// Uses keyed dependency injection to obtain the SQL Server storage instance.
/// </summary>
/// <remarks>
/// This class provides a concrete implementation of <see cref="AHfBackgroundJobMgr"/> 
/// with no additional functionality beyond what the base class offers. It exists
/// primarily to handle the dependency injection concerns and to provide a concrete
/// type that can be registered in the DI container.
/// <para>
/// Through its base class, it provides functionality for:
/// - Enqueuing fire-and-forget jobs
/// - Scheduling delayed jobs
/// - Requeuing jobs
/// - Rescheduling jobs
/// - Deleting jobs
/// </para>
/// <para>
/// This implementation uses the default queue specified in IdInfrastructureConstants.Jobs.Queues.Priority.
/// </para>
/// </remarks>
/// <param name="instanceProvider">Storage and Managers for Hangfire operations</param>
internal class HfPriorityBackgroundJobMgr(IHangfireInstanceProvider instanceProvider)
    : AHfBackgroundJobMgr(
        instanceProvider,
        IdInfrastructureConstants.Jobs.Queues.Priority),
    IHfPriorityBackgroundJobMgr
{ }
