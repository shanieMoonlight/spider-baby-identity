using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;

namespace ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;

/// <summary>
/// Interface for managing background jobs using Hangfire with a Priority queue.
/// </summary>
internal interface IHfDefaultBackgroundJobMgr : IHfBackgroundJobMgr;