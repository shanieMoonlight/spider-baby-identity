namespace ID.Infrastructure.Auth.JWT.Setup;

/// <summary>
/// Defines when refresh tokens should be updated/rotated during token refresh operations.
/// </summary>
public enum RefreshTokenUpdatePolicy
{
    /// <summary>
    /// Never update the refresh token - always return the same refresh token.
    /// </summary>
    Never = 0,

    /// <summary>
    /// Always update the refresh token on every refresh request.
    /// Provides maximum security but may impact user experience.
    /// </summary>
    Always = 1,

    /// <summary>
    /// Update the refresh token when it has lived 25% of its total lifetime.
    /// Provides good security with minimal user impact.
    /// </summary>
    QuarterLife = 2,

    /// <summary>
    /// Update the refresh token when it has lived 50% of its total lifetime.
    /// Balanced approach between security and user experience.
    /// </summary>
    HalfLife = 3,

    /// <summary>
    /// Update the refresh token when it has lived 75% of its total lifetime.
    /// More user-friendly while still providing reasonable security.
    /// </summary>
    ThreeQuarterLife = 4

}//Enm
