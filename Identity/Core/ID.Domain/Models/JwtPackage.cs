using ID.Domain.Utility.Dates;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IdentityModel.Tokens.Jwt;

namespace ID.Domain.Models;

/// <summary>
/// Class for encapsulating JWT
/// </summary>
public class JwtPackage
{

    /// <summary>
    /// Actual token
    /// </summary>
    public string AccessToken { get; private set; }

    /// <summary>
    /// Actual token
    /// </summary>
    public string? RefreshToken { get; private set; }

    /// <summary>
    /// How long token is valid for
    /// </summary>
    public long Expiration { get; private set; }

    /// <summary>
    /// Does the user need to verify themselves with a code as well
    /// </summary>
    public bool TwoStepVerificationRequired { get; private set; } = false;

    /// <summary>
    /// How will 2 factor be verified
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public TwoFactorProvider TwoFactorProvider { get; private set; } = TwoFactorProvider.Sms;

    /// <summary>
    /// Any details you need to add: I.E. Alternative 2-factor provider if first one fails 
    /// </summary>
    public string? ExtraInfo { get; private set; }

    //------------------------//

    #region Serialization Ctor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [JsonConstructor]
    private JwtPackage() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - //

    private JwtPackage(string accessToken) => AccessToken = accessToken;

    //- - - - - - - - - - - - //

    /// <summary>
    /// Creates a new instance of the <see cref="JwtPackage"/> class.
    /// Sets TwoStepVerificationRequired = false
    /// </summary>
    public static JwtPackage Create(string accessToken, long expiration, TwoFactorProvider twoFactorProvider, string? refreshToken) =>
        new(accessToken)
        {
            Expiration = expiration,
            TwoFactorProvider = twoFactorProvider,
            RefreshToken = refreshToken
        };

    //- - - - - - - - - - - - //

    /// <summary>
    /// Use when 2-Factor is required.
    /// Sets TwoStepVerificationRequired = true
    /// NO refresh token here. Wait until 2-Factor is verified
    /// </summary>
    /// <param name="provider"></param>
    public static JwtPackage CreateWithTwoFactoRequired(string accessToken, long expiration, TwoFactorProvider provider, string? extraInfo = null) =>
        new(accessToken)
        {
            Expiration = expiration,
            TwoFactorProvider = provider,
            TwoStepVerificationRequired = true,
            ExtraInfo = extraInfo
        };


    //------------------------//

    public DateTime ExpirationDate => Expiration.ConvertFromUnixTimestamp();

    //------------------------//

    public override string ToString() =>
        $"{ExpirationDate.ToLongDateString()}{Environment.NewLine}{AccessToken}";

    //------------------------//

    public JwtSecurityToken AsJwtSecurityToken()
    {
        var handler = new JwtSecurityTokenHandler();
        return (handler.ReadToken(AccessToken) as JwtSecurityToken)!;


    }

    //------------------------//

    //private static DateTime ConvertFromUnixTimestamp(long timestamp)
    //{
    //    DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    //    return origin.AddSeconds(timestamp);
    //}

}//Cls
