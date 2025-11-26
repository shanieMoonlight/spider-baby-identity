using ClArch.SimpleSpecification;
using ID.Domain.Entities.Refreshing;
using Microsoft.EntityFrameworkCore;
using StringHelpers;

namespace ID.Domain.Repos.Specs.RefreshTokens;

/// <summary>
/// Specification for retrieving a RefreshToken entity by its payload, including its associated User and Team.
/// </summary>
internal class RefreshTokenByPayloadWithUserAndDeviceAndTeamSpec : ASimpleSpecification<IdRefreshToken>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenByPayloadWithUserAndDeviceAndTeamSpec"/> class.
    /// </summary>
    /// <param name="tknPayload">The token payload to match.</param>
    public RefreshTokenByPayloadWithUserAndDeviceAndTeamSpec(string? tknPayload) : base(r => r.PayloadHash == tknPayload)
    {
        // Short-circuits the query if the token is null or whitespace.
        SetShortCircuit(() => tknPayload.IsNullOrWhiteSpace());

        // Includes the User and their associated Team in the query.
        SetInclude(query => query
            .Include(e => e.User)
                .ThenInclude(u => u!.Team)
            .Include(e => e.TrustedDevice)
        );
    }

    //-------------------------------------//

    /// <summary>
    /// Factory method to create a new instance of <see cref="RefreshTokenByPayloadWithUserAndDeviceAndTeamSpec"/>.
    /// </summary>
    /// <param name="tkn">The token payload to match.</param>
    /// <returns>A new instance of <see cref="RefreshTokenByPayloadWithUserAndDeviceAndTeamSpec"/>.</returns>
    public static RefreshTokenByPayloadWithUserAndDeviceAndTeamSpec Create(string? tkn) =>
        new(tkn);

}//Cls
