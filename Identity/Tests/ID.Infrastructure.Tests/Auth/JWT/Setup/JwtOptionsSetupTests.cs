using ID.GlobalSettings.Setup.Defaults;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Tests.Auth.JWT.Setup;

public class JwtOptionsSetupTests
{
    private const string _validPublicKeyXml = @"<RSAKeyValue>
<Modulus>1hYinUnmO1QyansNWEWin0JGA9fS0+MGlGi1WNHFDfm8eAiiT2KK11U8+gx+QjAqFNXMEv4TxPgIT99ANI+K5VD3/x7JsRJZTonmSg34WdAOSR2V/7blGWMkHjPwR5hiaNyMpOAwr0NcbIiNAtl6eubAG4kKkZ6c871S0Cmk1n4MoHZCMKJQ+CVAkOqy1TAtzZhgHxnQ86PGT7NbC37PqlC5G8Erwi81YT4Jqw0T7zFGo8wyWIeRm4JmmKVWVBSUr8qUC+QZF7th3TAO9PvCfaX5hW2wtOVlKZA9goTfzFICzr+Pxtq1dZC7sOmPJgp1kl/HLcCP020TF10m9dzG7w==</Modulus>
<Exponent>AQAB</Exponent>
</RSAKeyValue>";
    private const string _validPrivateKeyXml = @"<RSAKeyValue>
<Modulus>1hYinUnmO1QyansNWEWin0JGA9fS0+MGlGi1WNHFDfm8eAiiT2KK11U8+gx+QjAqFNXMEv4TxPgIT99ANI+K5VD3/x7JsRJZTonmSg34WdAOSR2V/7blGWMkHjPwR5hiaNyMpOAwr0NcbIiNAtl6eubAG4kKkZ6c871S0Cmk1n4MoHZCMKJQ+CVAkOqy1TAtzZhgHxnQ86PGT7NbC37PqlC5G8Erwi81YT4Jqw0T7zFGo8wyWIeRm4JmmKVWVBSUr8qUC+QZF7th3TAO9PvCfaX5hW2wtOVlKZA9goTfzFICzr+Pxtq1dZC7sOmPJgp1kl/HLcCP020TF10m9dzG7w==</Modulus>
<Exponent>AQAB</Exponent>
<P>9xcPskqb8ipjoBTGsqeXL6969RPQh5dUN+WC2auTcxiEWGXyHWB1yo7TmZRko8J1r42/zD6k5HKBmG/Gy1Zno+pTAKDkweJLtAXlyptNiMImfBYRReHXwsty/00fau2lVc61aRUt7sQiJATW/UY1jqErn1v3sL5/F1eGYeuJwPM=</P>
<Q>3c5pKsWuLs62xyeNQ4K/dXHpcNg3wNJT54u7+4Fkj+4ZQNV8kA8LKOvufHDsc8M8dPP084YdrTDmFKDmyU/taXcHrQe+5/JPX3pdl3ugkffLOffGgYD1GFdXqk5DZGVYapnktEbucI1ePKYyJPy/GFe/vFbDp0p8C9fZNxq1ARU=</Q>
<DP>hFfHwnkPuc9WeQFnw3zcD2Bv/SBVyqoVI7M8OJYbbcQt7qL74RwvOwTw9Qt0M/oNyq+jkSPkca+bFiiYU4S+Eh+JwYZrwCUS4yNdhv1Ts/I5ZrDzI3jpdZ4+w9ts/nq22ZTTuarsZTyMBLrK4/Fc8j4E/V/m9LWzoK7yfTQJHl0=</DP>
<DQ>vPF+7ruURDU8x+uuT0sKcy5FECZvX+cLKFwFFxrDIkRN6MezIzhdZk+MSR8cnQQ79Nh32hZuI0FbTUk/L0/RypxlwoStoAHukUO4hDkAsDcoPEoQI/NJVaHZgK7Ig7Y9GhncE6G0rdYO55UfdBiFZGQjZXl3k4NEpgYJ+AHdHH0=</DQ>
<InverseQ>sxuI06HLcck0L0ekygKvF/iKo/EutSs/OuTsifjaZ0KWFrobg3TRZEtgHvxZTtjJPHXSWBOkaY4aRG01jJK2Y1scuyzSPuBJpXMuhY8kCZjYLjastJPYFzloqiidROoKPl9DSY//ozYT1ByL1Bjd810252RrTHPseVTY3qZPmIo=</InverseQ>
<D>nN1TN5SyUb57wnGvcYJ0ieTxkFdPb1nltFCUsCPkEz1tzzXkV+6IdQdLypvk13KbIvEUusXYjnZ/AKdAUELtLuGJFTHl7wzWyylXx+M8mfJMxV4cTmYgr91o1YiRAqSxVsxjcVuj0Ie27P+Q8wmPKQZytLpROCnULvQF/ejFkzMpUqXgtzZm1KWKYOSiET2bWzIqnci3FwOBXXZbsJVs1HWnYaKQVNzCBLPF7yYy6MnypLAxj/zE4XITh4EONNUlu19anFbWL4U4ZfY1I+81gbxPwwvhDfpVCGk02iKnl38Zt+awU25rh7lEOKYcoreTzfTz3AwSuQ+LDZ1Hps9OIQ==</D>
</RSAKeyValue>";

    //--------------------------//

    [Fact]
    public void Sets_SymmetricKey_When_Provided_And_Valid()
    {
        var options = GetBaseOptions();
        options.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);
        var services = new ServiceCollection();
        var result = JwtOptionsSetup.ConfigureJwtOptions(services, options);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Throws_If_SymmetricKey_TooShort()
    {
        var options = GetBaseOptions();
        options.SymmetricTokenSigningKey = "short";
        var services = new ServiceCollection();
        Should.Throw<SetupDataException>(() => JwtOptionsSetup.ConfigureJwtOptions(services, options));
    }

    [Fact]
    public void Sets_AsymmetricPemKeyPair_When_Provided()
    {
        var options = GetBaseOptions();
        options.SymmetricTokenSigningKey = null;
        options.AsymmetricPemKeyPair = AsymmetricPemKeyPair.Create("-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7\n-----END PUBLIC KEY-----", "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQD7\n-----END PRIVATE KEY-----");
        var services = new ServiceCollection();
        var result = JwtOptionsSetup.ConfigureJwtOptions(services, options);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Sets_AsymmetricXmlKeyPair_When_Pem_Not_Provided()
    {
        var options = GetBaseOptions();
        options.SymmetricTokenSigningKey = null;
        options.AsymmetricPemKeyPair = null;
        options.AsymmetricXmlKeyPair = AsymmetricXmlKeyPair.Create(_validPublicKeyXml, _validPrivateKeyXml);
        var services = new ServiceCollection();
        var result = JwtOptionsSetup.ConfigureJwtOptions(services, options);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Throws_If_No_KeyPair_Provided()
    {
        var options = GetBaseOptions();
        options.SymmetricTokenSigningKey = null;
        options.AsymmetricPemKeyPair = null;
        options.AsymmetricXmlKeyPair = null;
        var services = new ServiceCollection();
        Should.Throw<SetupDataException>(() => JwtOptionsSetup.ConfigureJwtOptions(services, options));
    }

    [Fact]
    public void Uses_Defaults_For_Algorithm_And_Issuer()
    {
        var options = GetBaseOptions();
        options.TokenIssuer = null;
        options.SecurityAlgorithm = null;
        options.AsymmetricAlgorithm = null;
        options.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);
        var services = new ServiceCollection();
        var result = JwtOptionsSetup.ConfigureJwtOptions(services, options);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Sets_RefreshTokenPolicy_And_TimeSpan()
    {
        var options = GetBaseOptions();
        options.RefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.HalfLife;
        options.RefreshTokenTimeSpan = TimeSpan.FromMinutes(10);
        options.SymmetricTokenSigningKey = new string('a', IdGlobalDefaultValues.MIN_SYMMETRIC_SIGNING_KEY_LENGTH);
        var services = new ServiceCollection();
        var result = JwtOptionsSetup.ConfigureJwtOptions(services, options);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Sets_LegacyPemKeyPairs()
    {
        var options = GetBaseOptions();
        options.SymmetricTokenSigningKey = null;
        options.AsymmetricPemKeyPair = AsymmetricPemKeyPair.Create("-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7\n-----END PUBLIC KEY-----", "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQD7\n-----END PRIVATE KEY-----");
        options.LegacyAsymmetricPemKeyPairs = [AsymmetricPemKeyPair.Create("-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA8\n-----END PUBLIC KEY-----", "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQD8\n-----END PRIVATE KEY-----")];
        var services = new ServiceCollection();
        var result = JwtOptionsSetup.ConfigureJwtOptions(services, options);
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Sets_LegacyXmlKeyPairs_When_Pem_Not_Provided()
    {
        var options = GetBaseOptions();
        options.SymmetricTokenSigningKey = null;
        options.AsymmetricPemKeyPair = AsymmetricPemKeyPair.Create("-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7\n-----END PUBLIC KEY-----", "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQD7\n-----END PRIVATE KEY-----");
        options.LegacyAsymmetricPemKeyPairs = [];
        //options.LegacyAsymmetricXmlKeyPairs = [AsymmetricXmlKeyPair.Create("<RSAKeyValue><Modulus>vQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", "<RSAKeyValue><Modulus>vQ==</Modulus><Exponent>AQAB</Exponent><P>1A==</P><Q>2A==</Q><D>3A==</D></RSAKeyValue>")];
        options.LegacyAsymmetricXmlKeyPairs = [AsymmetricXmlKeyPair.Create(_validPublicKeyXml, _validPrivateKeyXml)];
        var services = new ServiceCollection();
        var result = JwtOptionsSetup.ConfigureJwtOptions(services, options);
        result.ShouldNotBeNull();
    }

    //--------------------------//

    private static IdInfrastructureSetupOptions GetBaseOptions() => new()
    {
        PasswordOptions = null!,
        SignInOptions = null!,
        TokenExpirationMinutes = 60,
        RefreshTokenTimeSpan = TimeSpan.FromMinutes(30),
        TokenIssuer = "issuer",
        RefreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.Always,
        SymmetricTokenSigningKey = null,
        SecurityAlgorithm = "alg",
        AsymmetricAlgorithm = "RS256",
        AsymmetricPemKeyPair = null,
        LegacyAsymmetricPemKeyPairs = [],
        AsymmetricXmlKeyPair = null,
        LegacyAsymmetricXmlKeyPairs = [],
        CookieLoginPath = null,
        CookieLogoutPath = null,
        CookieAccessDeniedPath = null,
        CookieSlidingExpiration = null,
        CookieExpireTimeSpan = null,
        SwaggerUrl = null,
        ExternalPages = [],
        AllowExternalPagesDevModeAccess = null,
        UseDbTokenProvider = null,
        ConnectionString = null
    };


}//Cls
