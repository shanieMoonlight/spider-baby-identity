using ClArch.SimpleSpecification;
using ID.Domain.Entities.Refreshing;
using Microsoft.EntityFrameworkCore;
using StringHelpers;

namespace ID.Domain.Repos.Specs.RefreshTokens;

/// <summary>
/// Specification for retrieving a RefreshToken entity by its selector, including its associated User and Team.
/// </summary>
internal class RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec : ASimpleSpecification<IdRefreshToken>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec"/> class.
    /// </summary>
    /// <param name="selector">The token selector to match.</param>
    public RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec(string? selector) : base(r => r.Selector == selector)
    {
        // Short-circuits the query if the selector is null or whitespace.
        SetShortCircuit(() => selector.IsNullOrWhiteSpace());

        // Includes the User and their associated Team in the query.
        SetInclude(query => query
            .Include(e => e.User)
                .ThenInclude(u => u!.Team)
            .Include(e => e.TrustedDevice)
        );
    }

    //-------------------------------------//

    /// <summary>
    /// Factory method to create a new instance of <see cref="RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec"/>.
    /// </summary>
    /// <param name="selector">The token selector to match.</param>
    /// <returns>A new instance of <see cref="RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec"/>.</returns>
    public static RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec Create(string? selector) =>
        new(selector);

}//Cls
