using ClArch.SimpleSpecification;
using ID.Domain.Entities.Refreshing;
using Microsoft.EntityFrameworkCore;
using StringHelpers;

namespace ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;

/// <summary>
/// Specification for retrieving a RefreshToken entity by its payload, including its associated User and Team.
/// </summary>
internal class RefreshTokenWithUserAndTeamSpec : ASimpleSpecification<IdRefreshToken>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenWithUserAndTeamSpec"/> class.
    /// </summary>
    /// <param name="tknPayload">The token payload to match.</param>
    public RefreshTokenWithUserAndTeamSpec(string? tknPayload) : base(r => r.Payload == tknPayload)
    {
        // Short-circuits the query if the token is null or whitespace.
        SetShortCircuit(() => tknPayload.IsNullOrWhiteSpace());

        // Includes the User and their associated Team in the query.
        SetInclude(query => query
            .Include(e => e.User)
                .ThenInclude(u => u!.Team)
                );
    }

    //-------------------------------------//

    /// <summary>
    /// Factory method to create a new instance of <see cref="RefreshTokenWithUserAndTeamSpec"/>.
    /// </summary>
    /// <param name="tkn">The token payload to match.</param>
    /// <returns>A new instance of <see cref="RefreshTokenWithUserAndTeamSpec"/>.</returns>
    public static RefreshTokenWithUserAndTeamSpec Create(string? tkn) =>
        new(tkn);

}//Cls
