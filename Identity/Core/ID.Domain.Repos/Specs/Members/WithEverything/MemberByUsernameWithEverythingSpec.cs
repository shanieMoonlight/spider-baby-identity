using ID.Domain.Entities.AppUsers;
using Microsoft.EntityFrameworkCore;

namespace ID.Domain.Repos.Specs.Members.WithEverything;

internal class MemberByUsernameWithEverythingSpec<TUser> 
    : AMemberWithEverythingSpec<TUser> where TUser : AppUser
{
    public MemberByUsernameWithEverythingSpec(string? username)
        : base(e =>
            e.UserName != null
            &&
            e.UserName.ToLower() == username!.ToLower()//! ShortCircuit will catch it.
        )
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(username));
        SetInclude(qry => qry
            .Include(u => u.TrustedDevices)
        );
    }

}//Cls
