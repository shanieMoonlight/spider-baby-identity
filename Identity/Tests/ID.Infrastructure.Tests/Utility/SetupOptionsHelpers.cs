using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Utility;
using ID.Infrastructure.Persistance.EF.Setup.Options;
using ID.Infrastructure.Setup;
using ID.Infrastructure.Setup.Options;
using ID.Infrastructure.Setup.Passwords;
using ID.Infrastructure.Setup.SignIn;

namespace ID.Infrastructure.Tests.Utility;
internal class SetupOptionsHelpers
{
    /// <summary>
    /// This should not cause errors so . If you set something to the wrong value only that should cause a n erro when testing
    /// </summary>
    /// <returns></returns>
    public static IdInfrastructureSetupOptions CreateValidDefaultSetupOptions()
    {
        return new IdInfrastructureSetupOptions
        {
            PasswordOptions = new IdPasswordOptions
            {
                RequiredLength = 6,
                RequiredUniqueChars = 1,
                RequireNonAlphanumeric = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireDigit = true
            },
            SignInOptions = new IdSignInOptions
            {
                RequireConfirmedEmail = false,
                RequireConfirmedPhoneNumber = false
            },
            AsymmetricTokenPublicKey_Xml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent></RSAKeyValue>",
            AsymmetricTokenPrivateKey_Xml = "<RSAKeyValue><Modulus>...</Modulus><Exponent>...</Exponent><P>...</P><Q>...</Q><DP>...</DP><DQ>...</DQ><InverseQ>...</InverseQ><D>...</D></RSAKeyValue>",
            AsymmetricTokenPrivateKey_Pem = null,
            AsymmetricTokenPublicKey_Pem = null,
            AsymmetricAlgorithm = IdGlobalDefaultValues.ASYMETRIC_ALGORITHM,
            SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH),
            TokenExpirationMinutes = IdGlobalDefaultValues.TOKEN_EXPIRATION_MINUTES,
            TokenIssuer = IdGlobalDefaultValues.TOKEN_ISSUER,
            SecurityAlgorithm = IdGlobalDefaultValues.SECURITY_ALGORITHM,
            UseDbTokenProvider = false,
            RefreshTokenTimeSpan = IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN,
            //RefreshTokensEnabled = IdInfrastructureDefaultValues.REFRESH_TOKENS_ENABLED,
            SwaggerUrl = InfrastructureDefaultValues.SWAGGER_URL,
            ExternalPages = [],
            ConnectionString = "SOmeConnectionToA Database",
            CookieLoginPath = "/Account/Login",
            CookieLogoutPath = "/Account/Logout",
            CookieAccessDeniedPath = "/Account/AccessDenied",
            CookieSlidingExpiration = true,
            CookieExpireTimeSpan = TimeSpan.FromMinutes(30),
            AllowExternalPagesDevModeAccess = true,
            RefreshTokenUpdatePolicy = Infrastructure.Auth.JWT.Setup.RefreshTokenUpdatePolicy.ThreeQuarterLife
        };
    }


    /// <summary>
    /// This should not cause errors so . If you set something to the wrong value only that should cause a n erro when testing
    /// </summary>
    /// <returns></returns>
    public static IdInfrastructureSetupOptions CreateEmptyDefaultSetupOptions()
    {
        return new IdInfrastructureSetupOptions
        {
            PasswordOptions = null,
            SignInOptions = null,
            AsymmetricTokenPublicKey_Xml = null,
            AsymmetricTokenPrivateKey_Xml = null,
            AsymmetricTokenPrivateKey_Pem = null,
            AsymmetricTokenPublicKey_Pem = null,
            AsymmetricAlgorithm = null,
            SymmetricTokenSigningKey = null,
            TokenExpirationMinutes = -1,
            TokenIssuer = null,
            SecurityAlgorithm =null,
            UseDbTokenProvider = false,
            RefreshTokenTimeSpan = IdGlobalDefaultValues.REFRESH_TOKEN_EXPIRE_TIME_SPAN,
            //RefreshTokensEnabled = IdInfrastructureDefaultValues.REFRESH_TOKENS_ENABLED,
            SwaggerUrl = null,
            ExternalPages = [],
            ConnectionString = null,
            CookieLoginPath = string.Empty,
            CookieLogoutPath = string.Empty,
            CookieAccessDeniedPath = string.Empty,
            CookieSlidingExpiration = true,
            CookieExpireTimeSpan = null,
            AllowExternalPagesDevModeAccess = true,
            RefreshTokenUpdatePolicy = null
        };
    }
}
