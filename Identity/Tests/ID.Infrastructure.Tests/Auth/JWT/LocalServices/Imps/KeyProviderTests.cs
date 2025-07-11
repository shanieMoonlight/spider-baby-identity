using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.LocalServices.Imps;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace ID.Infrastructure.Tests.Auth.JWT.LocalServices.Imps;

public class KeyProviderTests
{
  private const  string _publicKey1 = $@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1hYinUnmO1QyansNWEWi
n0JGA9fS0+MGlGi1WNHFDfm8eAiiT2KK11U8+gx+QjAqFNXMEv4TxPgIT99ANI+K
5VD3/x7JsRJZTonmSg34WdAOSR2V/7blGWMkHjPwR5hiaNyMpOAwr0NcbIiNAtl6
eubAG4kKkZ6c871S0Cmk1n4MoHZCMKJQ+CVAkOqy1TAtzZhgHxnQ86PGT7NbC37P
qlC5G8Erwi81YT4Jqw0T7zFGo8wyWIeRm4JmmKVWVBSUr8qUC+QZF7th3TAO9PvC
faX5hW2wtOVlKZA9goTfzFICzr+Pxtq1dZC7sOmPJgp1kl/HLcCP020TF10m9dzG
7wIDAQAB
-----END PUBLIC KEY-----";


    private const string _privateKey1 = $@"-----BEGIN RSA PRIVATE KEY-----
MIIEpQIBAAKCAQEA1hYinUnmO1QyansNWEWin0JGA9fS0+MGlGi1WNHFDfm8eAii
T2KK11U8+gx+QjAqFNXMEv4TxPgIT99ANI+K5VD3/x7JsRJZTonmSg34WdAOSR2V
/7blGWMkHjPwR5hiaNyMpOAwr0NcbIiNAtl6eubAG4kKkZ6c871S0Cmk1n4MoHZC
MKJQ+CVAkOqy1TAtzZhgHxnQ86PGT7NbC37PqlC5G8Erwi81YT4Jqw0T7zFGo8wy
WIeRm4JmmKVWVBSUr8qUC+QZF7th3TAO9PvCfaX5hW2wtOVlKZA9goTfzFICzr+P
xtq1dZC7sOmPJgp1kl/HLcCP020TF10m9dzG7wIDAQABAoIBAAcBB8l6kSg2vt5O
v7O4HE2c88DZ1XShYRk6fCUqpu/baeGWP4bDJ0SyB4H5jHXqfa31BeCtl4SVrGJX
xpHeclso23w18ll//q3I0C2mQfStdfiWM06ZTR4YAsBkNZ1Tn6b+zcq+tO9yKW3T
Lj3XuWyYH25AydI6zLeJGNKYa8ncNYL0gFPFeG7qvwwUt5mlVUkNyEbWCrZs8vTx
l5TQqmHeqqsCaxr8YLApzpS8NWu9wSZslI7y3oCSkahToDs1ejeqn71DVRohzlgd
Vbuyc0WK7VW8ioe19GoSGh6+AnXEHnBmE28cYm56nhrvagPB6ZW9DwXu0hG/yYTS
MRitfeUCgYEA9xcPskqb8ipjoBTGsqeXL6969RPQh5dUN+WC2auTcxiEWGXyHWB1
yo7TmZRko8J1r42/zD6k5HKBmG/Gy1Zno+pTAKDkweJLtAXlyptNiMImfBYRReHX
wsty/00fau2lVc61aRUt7sQiJATW/UY1jqErn1v3sL5/F1eGYeuJwPMCgYEA3c5p
KsWuLs62xyeNQ4K/dXHpcNg3wNJT54u7+4Fkj+4ZQNV8kA8LKOvufHDsc8M8dPP0
84YdrTDmFKDmyU/taXcHrQe+5/JPX3pdl3ugkffLOffGgYD1GFdXqk5DZGVYapnk
tEbucI1ePKYyJPy/GFe/vFbDp0p8C9fZNxq1ARUCgYEAhFfHwnkPuc9WeQFnw3zc
D2Bv/SBVyqoVI7M8OJYbbcQt7qL74RwvOwTw9Qt0M/oNyq+jkSPkca+bFiiYU4S+
Eh+JwYZrwCUS4yNdhv1Ts/I5ZrDzI3jpdZ4+w9ts/nq22ZTTuarsZTyMBLrK4/Fc
8j4E/V/m9LWzoK7yfTQJHl0CgYEAvPF+7ruURDU8x+uuT0sKcy5FECZvX+cLKFwF
FxrDIkRN6MezIzhdZk+MSR8cnQQ79Nh32hZuI0FbTUk/L0/RypxlwoStoAHukUO4
hDkAsDcoPEoQI/NJVaHZgK7Ig7Y9GhncE6G0rdYO55UfdBiFZGQjZXl3k4NEpgYJ
+AHdHH0CgYEAsxuI06HLcck0L0ekygKvF/iKo/EutSs/OuTsifjaZ0KWFrobg3TR
ZEtgHvxZTtjJPHXSWBOkaY4aRG01jJK2Y1scuyzSPuBJpXMuhY8kCZjYLjastJPY
FzloqiidROoKPl9DSY//ozYT1ByL1Bjd810252RrTHPseVTY3qZPmIo=
-----END RSA PRIVATE KEY-----";


    private const string _publicKey2 = $@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuF/a+kSUg3NuKhW7pk8E
T7UF4QUlxSg5drvJzwltVfwKDx7GCHQlUxJ7NyyxUIT7zB8SRlmVRZYv1Ng884xH
ihiuUVGaxTa2W0Q6kwyXWX8Px/m4+UjlPDd28ahN1fr+dvy7zp1x2c0SGxgBqJUE
mZ1QJAhxBUWze21dORaW9neBlCjoXdoZukeQ94h/Y3LKf0Q2G1ZMLHKhgF0eZn4q
o16G1+OUgH/XmoQhZK4d2R7LcGDDwis0/pcKTE2U2Xin67jS/k4A7DJq7EpqaUvx
h4OsKb8tmrjF5EJC5YQScMMtColERU6HQiPHGJ9N1Qd0SR8w1u3ouIveFeKN0AZw
7QIDAQAB
-----END PUBLIC KEY-----";


    private const string _privateKey2 = $@"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAuF/a+kSUg3NuKhW7pk8ET7UF4QUlxSg5drvJzwltVfwKDx7G
CHQlUxJ7NyyxUIT7zB8SRlmVRZYv1Ng884xHihiuUVGaxTa2W0Q6kwyXWX8Px/m4
+UjlPDd28ahN1fr+dvy7zp1x2c0SGxgBqJUEmZ1QJAhxBUWze21dORaW9neBlCjo
XdoZukeQ94h/Y3LKf0Q2G1ZMLHKhgF0eZn4qo16G1+OUgH/XmoQhZK4d2R7LcGDD
wis0/pcKTE2U2Xin67jS/k4A7DJq7EpqaUvxh4OsKb8tmrjF5EJC5YQScMMtColE
RU6HQiPHGJ9N1Qd0SR8w1u3ouIveFeKN0AZw7QIDAQABAoIBABD8CZLc/vZmC4u2
cVEmfc3u7eH3BK33IOYKchKPuoH9hBq0+ca9FlQjA3VHeXUirwR0h00yQvFEXtcj
bWf9L/jtgZ9tnk2VFvLs4914fzfIH/bKzyqtt6pWrK8h0zvHUao56X4k3GBVRxn2
We7C/Ye+KWwMWAcFjv1ri3cvGMyO+TgpRLy5z5lN7dASz6ABXmzriV94MG61c1xU
q9HPV7N0+z4sZR72joso/2o5eSU9R+s19f/lnEDrqcXBWvzKuAyuYo3OuRuHxENa
geS4W9+K2uWoKh+Ns8OBV8UFJ9j2jxxIaKvQBJvKnKHxRjGieFwQuN1NThRLKdfv
jSdZ/wkCgYEA3gPB+C6ZgxkpVDxQdF8m/iAHvBqjtHcNBIgTCEbgu8yjfv5wIkHI
H+brdyGxJT4/plKK4nbBCpOtyZdWtXM2Q2CPbH11VheFI2BsWSXmb57TpUhv4SZo
pNk2YV14UndYtDPqHqvDRMacsGJYT9VYRZ6tyLno86PcAMkw0964PKUCgYEA1JkQ
QmT5soPsFGJmVBtaa3qDivl0YV94q6lNFICbHGTTySj2TSMuAGoJB+vgBpQXGaWS
6oixjlm9xT4Bum2TZo0oTyQ3B3NFXjoiTiUZY63y/rpA7DB5lg6itXgwsFqsDm8H
gO1mOUHcPO0AvAtLvxyMD2o9SOMQj0MzSUJ4SKkCgYBxMotIxkHBRSf8ahA/dXCF
K8Gl2BNV5Ul+4P6LPtBBELLgAk0cOlwWX//4qvYOXjHH1Ng5ZYFBqlB2s7IjLA4j
xmz6TPFxpuZcRkKYJanbiaix4kAhFRtPyexfMnAx4+YXY0zgvIUAYR/tEweiFM5A
GQrUiG3NI+P9hpddv3ZCfQKBgQCQlDtre7IYBgFR541Bm4yFVT8KDxVndv7gdvV1
gPR7fdJNli7STJ9nJrVXjsC+mI2RInnkR+vALCWTctTIDObWMh78m1tVFL5TE2Pr
Eu3OTSjYtJ+cQGcfdnqOwNsTw8YI1tcahdgkDjcRs/fw6hmsMWTKwuxEmi7Tztac
9rhIaQKBgGbCfLDO4aDoH6nwxsalC43FQYAVBGHdvPHDl/fHDcxsFiXhkM2u+Rvb
nUppRFQIj4WvG52mOeeUylo9MXjX3QS8vlH7KG1xszrLqfmY40YmGMHvyVxKfL2j
iwDNkiCoslBntX27x1pvRlKchyhlkEn3R9ItZKvgcLi5Q5NJyxqZ
-----END RSA PRIVATE KEY-----";


    private static JwtOptions GetJwtOptions(bool useAsymmetric, AsymmetricPemKeyPair? current = null, List<AsymmetricPemKeyPair>? legacy = null)
    {
        return new JwtOptions
        {
            SymmetricTokenSigningKey= !useAsymmetric ? "mysupersecretkeymysupersecretkeymysupersecretkeymysupersecretkey12345" : "",
            CurrentAsymmetricKeyPair = current,
            LegacyAsymmetricKeyPairs = legacy ?? []
        };
    }

    private static AsymmetricPemKeyPair GetKeyPair(string pub = "PUBLIC", string priv = "PRIVATE")
        => AsymmetricPemKeyPair.Create(pub, priv);

    [Fact]
    public void BuildSymmetricSigningKey_Returns_SymmetricSecurityKey()
    {
        var options = Options.Create(GetJwtOptions(false));
        var kidBuilder = Mock.Of<IKeyIdBuilder>();
        var provider = new KeyProvider(options, kidBuilder);
        var key = provider.GetSymmetricSigningKey();
        key.ShouldBeOfType<SymmetricSecurityKey>();
    }

    [Fact]
    public void BuildValidationSigningKey_Returns_Symmetric_WhenNotAsymmetric()
    {
        var options = Options.Create(GetJwtOptions(false));
        var kidBuilder = Mock.Of<IKeyIdBuilder>();
        var provider = new KeyProvider(options, kidBuilder);
        var key = provider.GetValidationSigningKey();
        key.ShouldBeOfType<SymmetricSecurityKey>();
    }

    [Fact]
    public void BuildValidationSigningKey_Returns_Asymmetric_WhenAsymmetric()
    {
        var pair = GetKeyPair(_publicKey1, _privateKey1);
        var options = Options.Create(GetJwtOptions(true, pair));
        var kidBuilder = Mock.Of<IKeyIdBuilder>();
        var provider = new KeyProvider(options, kidBuilder);
        var key = provider.GetValidationSigningKey();
        key.ShouldBeOfType<RsaSecurityKey>();
    }

    [Fact]
    public void BuildValidationSigningKeys_Returns_AllKeys_Asymmetric()
    {
        var pair = GetKeyPair(_publicKey1, _privateKey1);
        var legacy = new List<AsymmetricPemKeyPair> { GetKeyPair(_publicKey2, _privateKey2) };
        var options = Options.Create(GetJwtOptions(true, pair, legacy));
        var kidBuilder = Mock.Of<IKeyIdBuilder>();
        var provider = new KeyProvider(options, kidBuilder);
        var keys = provider.GetValidationSigningKeys();
        keys.Count.ShouldBe(2);
        keys.ShouldAllBe(k => k is RsaSecurityKey);
    }

    [Fact]
    public void BuildValidationSigningKeys_Returns_OneKey_Symmetric()
    {
        var options = Options.Create(GetJwtOptions(false));
        var kidBuilder = Mock.Of<IKeyIdBuilder>();
        var provider = new KeyProvider(options, kidBuilder);
        var keys = provider.GetValidationSigningKeys();
        keys.Count.ShouldBe(1);
        keys[0].ShouldBeOfType<SymmetricSecurityKey>();
    }

    [Fact]
    public void ExportPublicKey_Returns_CurrentPublicKey()
    {
        var pair = GetKeyPair(_publicKey1, _privateKey1);
        var options = Options.Create(GetJwtOptions(true, pair));
        var kidBuilder = Mock.Of<IKeyIdBuilder>();
        var provider = new KeyProvider(options, kidBuilder);
        provider.ExportPublicKey().ShouldBe(_publicKey1);
    }

    [Fact]
    public void BuildPrivateRsaSigningKey_Throws_IfCurrentKeyPairNull()
    {
        var options = Options.Create(GetJwtOptions(true, null));
        var kidBuilder = Mock.Of<IKeyIdBuilder>();
        var provider = new KeyProvider(options, kidBuilder);
        Should.Throw<InvalidOperationException>(() => provider.GetPrivateRsaSigningKey());
    }
}
