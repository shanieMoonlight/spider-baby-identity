using ID.Domain.Entities.AppUsers;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;

internal class MemberByIdWithEverythingSpec<TUser> : AMemberlWithEverythingSpec<TUser> where TUser : AppUser
{
    public MemberByIdWithEverythingSpec(Guid? id)
        : base(e => e.Id == id)
    {
        SetShortCircuit(() => id == null);
    }

}//Cls
