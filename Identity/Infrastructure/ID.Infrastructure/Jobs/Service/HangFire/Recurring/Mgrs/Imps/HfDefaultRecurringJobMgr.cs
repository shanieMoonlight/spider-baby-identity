using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using ID.Infrastructure.Utility;


namespace ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Imps;

/// <summary>
/// Implementation of the recurring job manager that uses the Default Queue for Hangfire integration.
/// Uses keyed dependency injection to obtain the SQL Server storage instance.
/// </summary>
/// <remarks>
/// This class provides a concrete implementation of <see cref="AHfRecurringJobMgr"/> 
/// with no additional functionality beyond what the base class offers. It exists
/// primarily to handle the dependency injection concerns and to provide a concrete
/// type that can be registered in the DI container.
/// <para>
/// This implementation uses the default queue specified in IdInfrastructureConstants.Jobs.Queues.Default.
/// </para>
/// </remarks>
/// <param name="storage">SQL Server storage instance obtained via keyed dependency injection</param>
internal class HfDefaultRecurringJobMgr(IHangfireInstanceProvider storageProvider)
    : AHfRecurringJobMgr(
        storageProvider,
        IdInfrastructureConstants.Jobs.Queues.Default
    ),
    IHfDefaultRecurringJobMgr
{ }
