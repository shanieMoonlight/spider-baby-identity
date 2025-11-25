using ClArch.SimpleSpecification;
using ID.Domain.Entities.Refreshing;
using Microsoft.EntityFrameworkCore;

namespace ID.Domain.Repos.Specs.RefreshTokens;

/// <summary>
/// Specification for retrieving a RefreshToken entity by its payload, including its associated User and Team.
/// </summary>
internal class RefreshTokenByIdWithUserAndDeviceAndTeamSpec : ASimpleSpecification<IdRefreshToken>
{

    public Guid Seed { get; set; }

    //-----------------//

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenByIdWithUserAndDeviceAndTeamSpec"/> class.
    /// </summary>
    /// <param name="tknPayload">The token payload to match.</param>
    public RefreshTokenByIdWithUserAndDeviceAndTeamSpec(Guid? id) : base(r => r.Id == id)
    {
        // Short-circuits the query if the token is null or whitespace.
        SetShortCircuit(() => id == null);

        // Includes the User and their associated Team in the query.
        SetInclude(query => query
            .Include(e => e.User)
                .ThenInclude(u => u!.Team)
            .Include(e => e.TrustedDevice)
        );
    }

    //-----------------//

    /// <summary>
    /// Factory method to create a new instance of <see cref="RefreshTokenByIdWithUserAndDeviceAndTeamSpec"/>.
    /// </summary>
    /// <param name="tkn">The token payload to match.</param>
    /// <returns>A new instance of <see cref="RefreshTokenByIdWithUserAndDeviceAndTeamSpec"/>.</returns>
    public static RefreshTokenByIdWithUserAndDeviceAndTeamSpec Create(Guid? id) =>
        new(id);

}//Cls
