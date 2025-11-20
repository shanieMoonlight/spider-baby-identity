using ID.Domain.Entities.AppUsers;

namespace ID.Domain.Repos.Specs.Members.WithEverything;

internal class MemberByUsernameWithEverythingSpec<TUser> 
    : AMemberlWithEverythingSpec<TUser> where TUser : AppUser
{
    public MemberByUsernameWithEverythingSpec(string? username)
        : base(e =>
            e.UserName != null
            &&
            e.UserName.ToLower() == username!.ToLower()//! ShortCircuit will catch it.
        )
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(username));
    }

}//Cls
