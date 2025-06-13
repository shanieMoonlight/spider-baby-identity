using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Imps;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Utility;

namespace ID.Infrastructure.Tests.Jobs.Service.HF.Background;

public class HfPriorityBackgroundJobMgrTests : AHfBackgroundJobMgrTests
{
    public HfPriorityBackgroundJobMgrTests()
        : base(IdInfrastructureConstants.Jobs.Queues.Priority)
    {
    }

    internal override HfPriorityBackgroundJobMgr CreateJobManager(IHangfireInstanceProvider instanceProvider)
    {
        return new HfPriorityBackgroundJobMgr(instanceProvider);
    }

    [Fact]
    public override void JobManager_Should_ImplementCorrectInterface()
    {
        // Assert that this also implements IHfDefaultBackgroundJobMgr (as per your implementation)
        JobManager.ShouldBeAssignableTo<IHfPriorityBackgroundJobMgr>();
    }
}