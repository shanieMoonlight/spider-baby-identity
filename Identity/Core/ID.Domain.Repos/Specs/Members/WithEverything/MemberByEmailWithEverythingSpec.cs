using ID.Domain.Entities.AppUsers;
using Microsoft.EntityFrameworkCore;

namespace ID.Domain.Repos.Specs.Members.WithEverything;

internal class MemberByEmailWithEverythingSpec<TUser> : AMemberWithEverythingSpec<TUser> where TUser : AppUser
{
    public MemberByEmailWithEverythingSpec(string? email)
        : base(e =>
            e.Email != null
            &&
            e.Email == email!.ToLower()  //! ShortCircuit will catch it.
        )
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(email));
        SetInclude(qry => qry
            .Include(u => u.TrustedDevices)
        );
    }

}//Cls
