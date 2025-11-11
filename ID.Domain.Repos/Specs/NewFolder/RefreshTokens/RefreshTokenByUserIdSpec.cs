using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;

/// <summary>
/// Specification for retrieving refresh tokens by a specific user ID.
/// </summary>
/// <remarks>
/// This specification is used to filter refresh tokens based on the provided user ID.
/// It inherits from <see cref="ASimpleSpecification{TEntity}"/> and applies a criteria
/// to match the <see cref="IdRefreshToken.UserId"/> property with the given user ID.
/// </remarks>
internal class RefreshTokenByUserIdSpec(Guid userId)
    : ASimpleSpecification<IdRefreshToken>(r => r.UserId == userId)
{

    //---------------------------------//

    /// <summary>
    /// Creates a new instance of <see cref="RefreshTokenByUserIdSpec"/> using a user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose refresh tokens are to be retrieved.</param>
    /// <returns>A new instance of <see cref="RefreshTokenByUserIdSpec"/>.</returns>
    public static RefreshTokenByUserIdSpec Create(Guid userId) =>
        new(userId);

    //- - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Creates a new instance of <see cref="RefreshTokenByUserIdSpec"/> using an <see cref="AppUser"/>.
    /// </summary>
    /// <typeparam name="TUser">The type of the user, which must inherit from <see cref="AppUser"/>.</typeparam>
    /// <param name="user">The user whose refresh tokens are to be retrieved.</param>
    /// <returns>A new instance of <see cref="RefreshTokenByUserIdSpec"/>.</returns>
    public static RefreshTokenByUserIdSpec Create<TUser>(TUser user) where TUser : AppUser =>
        new(user.Id);

    //---------------------------------//

}//Cls
