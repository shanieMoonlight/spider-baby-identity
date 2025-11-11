using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;

#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
internal class MemberByEmailSpec<TUser> : ASimpleSpecification<TUser> where TUser : AppUser
{
    public MemberByEmailSpec(string? email)
        : base(e =>
            e.Email != null
            &&
            e.Email == email!.ToLower()  //! ShortCircuit will catch it.
        )
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(email));
    }

}//Cls

#pragma warning restore CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons