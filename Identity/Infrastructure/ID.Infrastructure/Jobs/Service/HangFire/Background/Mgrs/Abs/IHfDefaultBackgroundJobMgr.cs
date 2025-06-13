using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;

namespace ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;

/// <summary>
/// Interface for managing background jobs using Hangfire with a default queue.
/// </summary>
internal interface IHfPriorityBackgroundJobMgr : IHfBackgroundJobMgr;