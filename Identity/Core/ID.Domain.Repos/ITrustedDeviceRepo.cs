using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Repos.GenRepo;

namespace ID.Domain.Repos;

internal interface IIdentityTrustedDeviceRepo : IGenCrudRepo<TrustedDevice>
{
}
