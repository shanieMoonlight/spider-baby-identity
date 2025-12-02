using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Repos.Specs.Members.WithEverything;

internal class MemberByIdWithEverythingSpec<TUser> : AMemberWithEverythingSpec<TUser> where TUser : AppUser
{
    public MemberByIdWithEverythingSpec(Guid? id)
        : base(e => e.Id == id)
    {
        SetShortCircuit(() => id == null);

    }

}//Cls
