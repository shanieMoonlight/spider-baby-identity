namespace ID.Application.AppAbs.TrustedDevices;
internal interface ITrustedDeviceFinder
{
    Task<GenResult<TrustedDevice>> FindWithUserAndTeamAsync(Guid deviceId, Guid userId);
}