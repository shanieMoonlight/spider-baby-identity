using ID.Domain.Entities.AppUsers;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;

internal class MemberByEmailWithEverythingSpec<TUser> : AMemberlWithEverythingSpec<TUser> where TUser : AppUser
{
    public MemberByEmailWithEverythingSpec(string? email)
        : base(e =>
            e.Email != null
            &&
            e.Email == email!.ToLower()  //! ShortCircuit will catch it.
        )
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(email));
    }

}//Cls
