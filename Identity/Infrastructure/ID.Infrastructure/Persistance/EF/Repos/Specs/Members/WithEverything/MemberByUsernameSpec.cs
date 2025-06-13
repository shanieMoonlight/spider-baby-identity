using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;

internal class MemberByUsernameSpec<TUser> 
    : ASimpleSpecification<TUser> where TUser : AppUser
{
    public MemberByUsernameSpec(string? username)
        : base(e =>
            e.UserName != null
            &&
            e.UserName.ToLower() == username!.ToLower()//! ShortCircuit will catch it.
        )
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(username));
    }

}//Cls
