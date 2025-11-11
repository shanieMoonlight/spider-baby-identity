using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Members;

internal class MemberExistsIdSpec<TUser> 
    : ASimpleSpecification<TUser> where TUser : AppUser
{
    public MemberExistsIdSpec(Guid? id)
        : base(e => e.Id == id)
    {
        SetShortCircuit(() => id == null);
    }
}
