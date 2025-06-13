using Hangfire;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Jobs.Service.HangFire.Instance.Imps;
[method: ActivatorUtilitiesConstructor]
internal class HangfireInstanceProvider(
    [FromKeyedServices(IdInfrastructureConstants.Jobs.DI_StorageKey)] JobStorage storage) : IHangfireInstanceProvider
{
    public JobStorage Storage { get; private set; } = storage;
    public IRecurringJobManagerWrapper RecurringJobManager { get; private set; } = new RecurringJobManagerWrapper(storage);
    public IBackgroundJobClientWrapper BackgroundJobClient { get; private set; } = new BackgroundJobClientWrapper(storage);
}
