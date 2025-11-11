using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Members;

internal class MemberExistsEmailSpec<TUser>
    : ASimpleSpecification<TUser> where TUser : AppUser
{
    public MemberExistsEmailSpec(string? email)
        : base(e => e.Email!.ToLower() == email!.ToLower())
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(email));
    }
}
