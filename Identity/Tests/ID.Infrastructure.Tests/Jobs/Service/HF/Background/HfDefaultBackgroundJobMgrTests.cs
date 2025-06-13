using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Imps;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Utility;

namespace ID.Infrastructure.Tests.Jobs.Service.HF.Background;

public class HfDefaultBackgroundJobMgrTests : AHfBackgroundJobMgrTests
{
    public HfDefaultBackgroundJobMgrTests()
        : base(IdInfrastructureConstants.Jobs.Queues.Default)
    {
    }

    internal override HfDefaultBackgroundJobMgr CreateJobManager(IHangfireInstanceProvider instanceProvider)
    {
        return new HfDefaultBackgroundJobMgr(instanceProvider);
    }

    [Fact]
    public override void JobManager_Should_ImplementCorrectInterface()
    {
        // Assert
        JobManager.ShouldBeAssignableTo<IHfDefaultBackgroundJobMgr>();
    }
}