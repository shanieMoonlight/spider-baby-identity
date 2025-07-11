using ID.Infrastructure.Auth.JWT.Setup;
using TestingHelpers;

namespace ID.Infrastructure.Tests.Auth.JWT.Utils;
public class JwtOptionsUtils
{
    public static JwtOptions ValidOptions => new()
    {

        TokenExpirationMinutes = 30,
        RefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.ThreeQuarterLife,
        RefreshTokenTimeSpan = TimeSpan.FromDays(2),
        SymmetricTokenSigningKey = RandomStringGenerator.Generate(64),
        TokenIssuer = "TestIssuer",
        SecurityAlgorithm = "HS256",
        //AsymmetricTokenPublicKey_Xml = string.Empty,
        AsymmetricAlgorithm = "RS256",
        //AsymmetricTokenPrivateKey_Xml = string.Empty,
        //AsymmetricTokenPrivateKey_Pem = string.Empty,
        //AsymmetricTokenPublicKey_Pem = string.Empty
    };

    //------------------------------//


    public static JwtOptions InitiallyValidOptions(
        int? tokenExpirationMinutes = null,
        RefreshTokenUpdatePolicy? refreshTokenUpdatePolicy = null,
        string? symmetricTokenSigningKey = null,
        string? tokenIssuer = null,
        string? securityAlgorithm = null,
        string? asymmetricAlgorithm = null,
        TimeSpan? refreshTokenTimeSpan = null)
    {
        return new()
        {
            TokenExpirationMinutes = tokenExpirationMinutes ?? 30,
            RefreshTokenUpdatePolicy = refreshTokenUpdatePolicy??  RefreshTokenUpdatePolicy.ThreeQuarterLife,
            RefreshTokenTimeSpan =refreshTokenTimeSpan ?? TimeSpan.FromDays(2),
            SymmetricTokenSigningKey =symmetricTokenSigningKey ?? string.Empty, 
            TokenIssuer = tokenIssuer??"TestIssuer",
            SecurityAlgorithm = securityAlgorithm??"HS256",
            AsymmetricAlgorithm = asymmetricAlgorithm ?? "RS256",
            //AsymmetricTokenPublicKey_Xml =asymmetricTokenPublicKey_Xml?? string.Empty,
            //AsymmetricTokenPrivateKey_Xml = asymmetricTokenPrivateKey_Xml ?? string.Empty,
            //AsymmetricTokenPrivateKey_Pem = asymmetricTokenPrivateKey_Pem ?? string.Empty,
            //AsymmetricTokenPublicKey_Pem = asymmetricTokenPublicKey_Pem ?? string.Empty
        };
    }


}
