using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ID.GlobalSettings.JWT;

/// <summary>
/// Class for encapsulating the results of a token validation attempt.
/// </summary>
public class JwtValidationResult
{

    /// <summary>
    /// Create successful validation object
    /// </summary>
    /// <param name="token">Validated token</param>
    public static JwtValidationResult Failure(SecurityTokenValidationException? exception) =>
        new()
        {
            Succeeded = false,
            Exception = exception
        };

    //------------------------------------//

    /// <summary>
    /// Create failed validation object
    /// </summary>
    /// <param name="exception">Exception detailing failure</param>
    public static JwtValidationResult Success(JwtSecurityToken token) =>
        new()
        {
            Token = token,
            Succeeded = true
        };

    //------------------------------------//

    /// <summary>
    /// Whether the Validation succeeded or failed.
    /// </summary>
    public bool Succeeded { get; private set; }

    /// <summary>
    /// Validated Token
    /// </summary>
    public JwtSecurityToken? Token { get; private set; }

    /// <summary>
    /// Details of validation failure.
    /// </summary>
    public SecurityTokenValidationException? Exception { get; private set; }


}//Cls
