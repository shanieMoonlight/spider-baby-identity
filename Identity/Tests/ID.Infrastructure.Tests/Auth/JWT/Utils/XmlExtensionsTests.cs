using ID.Infrastructure.Auth.JWT.Utils;
using Microsoft.IdentityModel.Tokens;
using System.Xml;

namespace ID.Infrastructure.Tests.Auth.JWT.Utils;

public class XmlExtensionsTests
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
    private const string _invalidXml = "<notxml>";
    private const string _missingElementsXml = @"<RSAKeyValue><Modulus>vQ==</Modulus></RSAKeyValue>";


    //--------------------------//


    [Fact]
    public void IsValidRsaPublicKeyXml_ReturnsTrue_ForValidPublicKey()
    {
        Assert.True(_validPublicKeyXml.IsValidRsaPublicKeyXml());
    }


    //--------------------------//

    [Fact]
    public void IsValidRsaPublicKeyXml_ReturnsFalse_ForInvalidXml()
    {
        Assert.False(_invalidXml.IsValidRsaPublicKeyXml());
    }


    //--------------------------//

    [Fact]
    public void IsValidRsaPublicKeyXml_ReturnsFalse_ForMissingElements()
    {
        Assert.False(_missingElementsXml.IsValidRsaPublicKeyXml());
    }


    //--------------------------//

    [Fact]
    public void IsValidRsaPrivateKeyXml_ReturnsTrue_ForValidPrivateKey()
    {
        Assert.True(_validPrivateKeyXml.IsValidRsaPrivateKeyXml());
    }


    //--------------------------//

    [Fact]
    public void IsValidRsaPrivateKeyXml_ReturnsFalse_ForInvalidXml()
    {
        Assert.False(_invalidXml.IsValidRsaPrivateKeyXml());
    }


    //--------------------------//


    [Fact]
    public void IsValidRsaPrivateKeyXml_ReturnsFalse_ForMissingElements()
    {
        Assert.False(_validPublicKeyXml.IsValidRsaPrivateKeyXml());
    }

    //--------------------------//

    [Fact]
    public void GetRSAParameters_ReturnsParameters_ForValidXml()
    {
        var parameters = _validPublicKeyXml.GetRSAParameters();
        Assert.NotNull(parameters.Modulus);
        Assert.NotNull(parameters.Exponent);
    }


    //--------------------------//


    [Fact]
    public void GetRSAParameters_Throws_ForInvalidXml()
    {
        Assert.Throws<XmlException>(() => _invalidXml.GetRSAParameters());
    }

    //--------------------------//

    [Fact]
    public void BuildRsaSigningKey_ReturnsKey_ForValidXml()
    {
        var key = _validPublicKeyXml.BuildRsaSigningKey();
        Assert.IsType<RsaSecurityKey>(key);
    }


    //--------------------------//


    [Fact]
    public void BuildRsaSigningKey_Throws_ForInvalidXml()
    {
        Assert.Throws<XmlException>(() => _invalidXml.BuildRsaSigningKey());
    }

}//Cls
