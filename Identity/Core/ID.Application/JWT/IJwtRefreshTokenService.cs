using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;

namespace ID.Application.JWT;

public interface IJwtRefreshTokenService<TUser> where TUser : AppUser
{
    /// <summary>
    /// Finds a refresh token along with its associated user and team.
    /// </summary>
    /// <param name="tknPayload">The refresh token payload.</param>
    /// <returns>The refresh token with user and team information, or null if not found.</returns>
    Task<IdRefreshToken?> FindTokenWithUserAndTeamAsync(string tknPayload, CancellationToken cancellationToken = default);

    //-------------------------//

    /// <summary>
    /// Generates a new refresh token for the specified user. And stores it.
    /// </summary>
    /// <param name="user">The user for whom the token is being generated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The newly created refresh token.</returns>
    Task<IdRefreshToken> GenerateTokenAsync(TUser user, CancellationToken cancellationToken);

    //-------------------------//

    /// <summary>
    /// Updates the payload of an existing refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated refresh token.</returns>
    Task<IdRefreshToken> UpdateTokenPayloadAsync(IdRefreshToken refreshToken, CancellationToken cancellationToken = default);

    //-------------------------//

    /// <summary>
    /// Revokes all refresh tokens associated with the specified user.
    /// </summary>
    /// <param name="user">The user whose tokens are to be revoked.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task RevokeTokensAsync(TUser user, CancellationToken cancellationToken = default);
}
