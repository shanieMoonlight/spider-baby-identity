using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Imps;
using ID.Infrastructure.Utility;

namespace ID.Infrastructure.Tests.Jobs.Service.HF.Recurring;

public class HfDefaultRecurringJobMgrTests : AHfRecurringJobMgrTests
{
    public HfDefaultRecurringJobMgrTests() 
        : base(IdInfrastructureConstants.Jobs.Queues.Default)
    {
    }

    internal override HfDefaultRecurringJobMgr CreateJobManager(IHangfireInstanceProvider instanceProvider)
    {
        return new HfDefaultRecurringJobMgr(instanceProvider);
    }

    [Fact]
    public override void JobManager_Should_ImplementCorrectInterface()
    {
        // Assert
        JobManager.ShouldBeAssignableTo<IHfDefaultRecurringJobMgr>();
    }


}