using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Repos;
using ID.Persistence.Ef.Repos.Abstractions;
using MyResults;

namespace ID.Persistence.Ef.Repos;
internal class TrustedDeviceRepo(IdDbContext db) : AGenCrudRepo<TrustedDevice>(db), IIdentityTrustedDeviceRepo
{
    protected override Task<BasicResult> CanDeleteAsync(TrustedDevice? dbDevice)
    {
        if (dbDevice is null)
            return Task.FromResult(BasicResult.Success());

        return Task.FromResult(BasicResult.Success());
    }

}//Cls
