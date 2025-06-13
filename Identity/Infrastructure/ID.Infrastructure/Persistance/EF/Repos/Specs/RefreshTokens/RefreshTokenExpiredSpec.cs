using ClArch.SimpleSpecification;
using ID.Domain.Entities.Refreshing;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;

/// <summary>
/// Specification for retrieving expired refresh tokens (tokens where ExpiresOnUtc is earlier than current date/time).
/// </summary>
internal class RefreshTokenExpiredSpec : ASimpleSpecification<IdRefreshToken>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenExpiredSpec"/> class.
    /// </summary>
    public RefreshTokenExpiredSpec() : base(r => r.ExpiresOnUtc < DateTime.UtcNow)
    { }

    //---------------------------------//

    public static RefreshTokenExpiredSpec Create() =>
        new();

    //---------------------------------//

}//Cls
